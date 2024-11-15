using Dapper;
using Microsoft.Data.Sqlite;
using Polly;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Repository.Interfaces;

namespace Test.Repository
{
    public class DbHelperSqlite : IDbHelper
    {
        #region Private Fields

        private static SqliteConnection conn;

        private static int maxRetryCount = 3;
        // strategy

        #endregion Private Fields

        #region Public Delegates + Events

        //public delegate void SqlChangeEventHandler(object sender, SqlChangeEventArgs e);

        public delegate void SqlSubscribeErrorHandler(object sender, Exception exception);

        #endregion Public Delegates + Events

        #region Private Methods

        //-------- this is one approch for the retry logic handling
        public async Task<T?> InsertRecordAsync<T>(
        string connectionString,
        string sql,
        object paramList,
        CancellationToken? cancellationToken = null
        )
        {
            var retryPolicy = Policy
                .Handle<SqliteException>()  // Specify the type of exception to handle
                .WaitAndRetryAsync(maxRetryCount, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))); // Exponential backoff

            return await retryPolicy.ExecuteAsync(async ct =>
            {
                using var connection = new SqliteConnection(connectionString);
                await connection.OpenAsync(ct); // Pass the cancellation token
                try
                {
                    // Perform the database operation            
                    var result = await connection.ExecuteScalarAsync<T>(
                        sql,
                        param: paramList,
                        commandType: CommandType.Text
                        ); // Pass the cancellation token

                    return result;
                }
                catch (SqliteException)
                {
                    // Rethrow the exception to let Polly handle the retry logic
                    throw;
                }
            }, cancellationToken ?? CancellationToken.None); // Ensure cancellationToken is not null
        }
        //------- end 1st approch of the retry policy handling
        /// <summary>
        /// Handle sql call
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private  async Task<T> HandleSqlCall<T>(string connectionString, Func<SqliteConnection, Task<T>> func)
        {
            return await Policy
                          // sql deadlock resource (https://sqlbak.com/academy/transaction-process-id-was-deadlocked-on-lock-resources-with-another-process-and-has-been-chosen-as-the-deadlock-victim-msg-1205)
                          //.Handle<SQLiteException>(ex => ex.Number == 1205)
                          .Handle<SqliteException>(ex => ex.ErrorCode == 1205)
                          .RetryAsync(3)
                          .ExecuteAsync(async () =>
                          {
                              using (SqliteConnection cn = new SqliteConnection(connectionString))
                              {
                                  try
                                  {
                                      await cn.OpenAsync().ConfigureAwait(false);

                                      return await func.Invoke(cn);
                                  }
                                  catch (Exception ex)
                                  {
                                      string s = ex.GetBaseException().Message;
                                      throw;
                                  }
                              }
                          }).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe for changes
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="timeOut"></param>
        /// <param name="onChange"></param>
        private void SubscribeForChanges(string commandText, CommandType commandType, int timeOut)
        {
            try
            {
                TimeSpan pauseBetweenFailures = TimeSpan.FromSeconds(30);

                Polly.Retry.RetryPolicy retryPolicy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryForever(retryAttempt =>
                        pauseBetweenFailures
                    );

                retryPolicy.Execute(() =>
                {
                    Subscribe(commandText, commandType, timeOut);
                });
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Subscribe to command
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="timeOut"></param>
        /// <param name="onChange"></param>
        private void Subscribe(string commandText, CommandType commandType, int timeOut)
        {
            try
            {
                // always new object
                OpenConnection();
                SqliteCommand comm = new SqliteCommand(commandText, conn)
                {
                    CommandType = commandType,
                    CommandTimeout = timeOut
                };
                /*SqliteDependency dependency = new SqlDependency(comm);
                // Subscribe to the SqlDependency event.
                dependency.OnChange += new OnChangeEventHandler(onChange);
                */

                // Subscribe to the SqlDependency event.
                //this is commented in sqlite  dependency.OnChange += new OnChangeEventHandler(onChange);

                // Execute the command.
                using (SqliteDataReader reader = comm.ExecuteReader())
                {
                    //do nothing, we use watcher only for change notification
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Open connection
        /// </summary>
        private static void OpenConnection()
        {
            if (conn?.State != ConnectionState.Open)
                conn.Open();
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Stops current sql notification
        /// </summary>
        public void StopDependencyWatcher(string connectionString)
        {
            //SqlDependency.Stop(connectionString);
        }

        /// <summary>
        /// Start sql notifications on command
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="onChange"></param>
        /// <param name="sqlSubscribeError"></param>
        /// <param name="timeOut"></param>
        private void StartDependencyWatcher(string connectionString, string commandText, CommandType commandType, SqlSubscribeErrorHandler sqlSubscribeError, int timeOut = 30)
        {
            try
            {
                TimeSpan pauseBetweenFailures = TimeSpan.FromSeconds(30);

                Polly.Retry.RetryPolicy retryPolicy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryForever(retryAttempt =>
                        pauseBetweenFailures
                    );

                retryPolicy.Execute(() =>
                {
                    try
                    {
                        //SqlDependency.Start(connectionString);

                        if (conn == null)
                        {
                            conn = new SqliteConnection(connectionString);
                            conn.Open();
                        }

                        SubscribeForChanges(commandText, commandType, timeOut);
                    }
                    catch (Exception ex)
                    {
                        sqlSubscribeError?.Invoke(null, ex);
                        // do nothing, polly will retry
                        throw;
                    }
                });
            }
            catch (Exception exception)
            {
                sqlSubscribeError?.Invoke(null, exception);
                throw;
            }
        }

        /// <summary>
        /// Execute a stored procedure and get a return object
        /// </summary>
        /// <param name="procedureName">name of the procedure</param>
        /// <param name="paramList">dynamic typed list of parameters</param>
        /// <returns></returns>
        public IEnumerable<T> QueryStoredProcedure<T>(string procedureName, object paramList,
            string connectionString, int timeOut = 60) where T : class
        {
            using (IDbConnection cn = new SqliteConnection(connectionString))
            {
                return cn.Query<T>($"[dbo].{procedureName}",
                    param: paramList,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: timeOut);
            }
        }

        /// <summary>
        /// Retrieve multiple recordsets async
        /// </summary>
        /// <typeparam name="T">type 1</typeparam>
        /// <typeparam name="T2">type 2</typeparam>
        /// <param name="procedureName">name of stored procedure</param>
        /// <param name="paramList">parameters</param>
        /// <param name="connectionString">connectionstring</param>
        /// <returns></returns>
        public async Task<(IEnumerable<T>, IEnumerable<T2>)> QueryMultipleRecordsetsAsync<T, T2>(string procedureName, object paramList, string connectionString, int timeOut = 30)
        {
            // Single exception type with condition
            return await HandleSqlCall(connectionString, async (cn) =>
            {
                //using (IDbConnection cn = new SqlConnection(connectionString))
                //{
                using (SqlMapper.GridReader result = await cn.QueryMultipleAsync($"[dbo].{procedureName}",
                param: paramList,
                commandType: CommandType.StoredProcedure,
                commandTimeout: timeOut))
                {
                    IEnumerable<T> table1 = await result.ReadAsync<T>();
                    IEnumerable<T2> table2 = null;
                    if (!result.IsConsumed)
                    {
                        table2 = await result.ReadAsync<T2>();
                    }

                    return (table1, table2);
                }
            });
        }

        /// <summary>
        /// Retrieve multiple recordsets async
        /// </summary>
        /// <typeparam name="T">type 1</typeparam>
        /// <typeparam name="T2">type 2</typeparam>
        /// <typeparam name="T3">type 3</typeparam>
        /// <param name="procedureName">name of stored procedure</param>
        /// <param name="paramList">parameters</param>
        /// <param name="connectionString">connectionstring</param>
        /// <returns></returns>
        public async Task<(IEnumerable<T>, IEnumerable<T2>, IEnumerable<T3>)> QueryMultipleRecordsetsAsync<T, T2, T3>(string procedureName, object paramList, string connectionString, int timeOut = 30)
        {
            // Single exception type with condition
            return await HandleSqlCall(connectionString, async (cn) =>
            {
                SqlMapper.GridReader result = await cn.QueryMultipleAsync($"[dbo].{procedureName}",
                    param: paramList,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: timeOut);

                IEnumerable<T> table1 = await result.ReadAsync<T>();
                IEnumerable<T2> table2 = await result.ReadAsync<T2>();
                IEnumerable<T3> table3 = await result.ReadAsync<T3>();

                return (table1, table2, table3);
            });
        }

        /// <summary>
        /// Retrieve multiple recordsets async
        /// </summary>
        /// <typeparam name="T">type 1</typeparam>
        /// <typeparam name="T2">type 2</typeparam>
        /// <typeparam name="T3">type 3</typeparam>
        /// <typeparam name="T4">type 4</typeparam>
        /// <param name="procedureName">name of stored procedure</param>
        /// <param name="paramList">parameters</param>
        /// <param name="connectionString">connectionstring</param>
        /// <returns></returns>
        public async Task<(IEnumerable<T>, IEnumerable<T2>, IEnumerable<T3>,
            IEnumerable<T4>)> QueryMultipleRecordsetsAsync<T, T2, T3, T4>(string procedureName, object paramList, string connectionString, int timeOut = 30)
        {
            // Single exception type with condition
            return await HandleSqlCall(connectionString, async (cn) =>
            {
                SqlMapper.GridReader result = await cn.QueryMultipleAsync($"[dbo].{procedureName}",
                    param: paramList,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: timeOut);

                IEnumerable<T> table1 = await result.ReadAsync<T>();
                IEnumerable<T2> table2 = await result.ReadAsync<T2>();
                IEnumerable<T3> table3 = await result.ReadAsync<T3>();
                IEnumerable<T4> table4 = await result.ReadAsync<T4>();

                return (table1, table2, table3, table4);
            });
        }

        /// <summary>
        /// Retrieve multiple recordsets async
        /// </summary>
        /// <typeparam name="T">type 1</typeparam>
        /// <typeparam name="T2">type 2</typeparam>
        /// <typeparam name="T3">type 3</typeparam>
        /// <typeparam name="T4">type 4</typeparam>
        /// <typeparam name="T5">type 5</typeparam>
        /// <param name="procedureName">name of stored procedure</param>
        /// <param name="paramList">parameters</param>
        /// <param name="connectionString">connectionstring</param>
        /// <returns></returns>
        public async Task<(IEnumerable<T>, IEnumerable<T2>, IEnumerable<T3>,
            IEnumerable<T4>, IEnumerable<T5>)> QueryMultipleRecordsetsAsync<T, T2, T3, T4, T5>(string procedureName, object paramList, string connectionString, int timeOut = 30)
        {
            using (IDbConnection cn = new SqliteConnection(connectionString))
            {
                SqlMapper.GridReader result = await cn.QueryMultipleAsync($"[dbo].{procedureName}",
                    param: paramList,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: timeOut);

                IEnumerable<T> table1 = await result.ReadAsync<T>();
                IEnumerable<T2> table2 = await result.ReadAsync<T2>();
                IEnumerable<T3> table3 = await result.ReadAsync<T3>();
                IEnumerable<T4> table4 = await result.ReadAsync<T4>();
                IEnumerable<T5> table5 = await result.ReadAsync<T5>();

                return (table1, table2, table3, table4, table5);
            }
        }

        /// <summary>
        /// Execute a stored procedure and get a return object
        /// </summary>
        /// <param name="procedureName">name of the procedure</param>
        /// <param name="paramList">dynamic typed list of parameters</param>
        /// <returns></returns>
        public T Scalar<T>(string procedureName, object paramList,
            string connectionString)
        {
            using (IDbConnection cn = new SqliteConnection(connectionString))
            {
                return cn.ExecuteScalar<T>($"[dbo].{procedureName}",
                    param: paramList,
                    commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// Execute a stored procedure
        /// </summary>
        /// <param name="procedureName">name of the procedure</param>
        /// <param name="paramList">dynamic typed list of parameters</param>
        /// <returns></returns>
        public void ExecuteStoredProcedure(string procedureName, object paramList,
            string connectionString)
        {
            using (IDbConnection cn = new SqliteConnection(connectionString))
            {
                cn.Execute($"[dbo].{procedureName}",
                    param: paramList,
                    commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// Execute a stored procedure and get a return object
        /// </summary>
        /// <param name="procedureName">name of the procedure</param>
        /// <param name="paramList">dynamic typed list of parameters</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> QueryStoredProcedureAsync<T>(string procedureName, object paramList,
            string connectionString, int timeOut = 30) where T : class
        {
            // Single exception type with condition
            return await HandleSqlCall(connectionString, async (cn) =>
            {
                return await cn.QueryAsync<T>($"[dbo].{procedureName}",
                    param: paramList,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: timeOut);
            });
        }

        /// <summary>
        /// Execute a stored procedure and get a return object
        /// </summary>
        /// <param name="procedureName">name of the procedure</param>
        /// <param name="paramList">dynamic typed list of parameters</param>
        /// <returns></returns>
        public async Task<T?> QueryStoredProcedureFirstAsync<T>(string procedureName, object paramList,
            string connectionString) where T : class
        {
            // Single exception type with condition
            return await HandleSqlCall(connectionString, async (cn) =>
            {
                return await cn.QueryFirstOrDefaultAsync<T>($"[dbo].{procedureName}",
                    param: paramList,
                    commandType: CommandType.StoredProcedure);
            });
        }

        /// <summary>
        /// Execute a stored procedure and get a return object
        /// </summary>
        /// <param name="procedureName">name of the procedure</param>
        /// <param name="paramList">dynamic typed list of parameters</param>
        /// <param name="timeOut">time out</param>
        /// <returns></returns>
        public async Task<T?> QueryStoredProcedureFirstAsync<T>(string procedureName, object paramList,
            string connectionString, int timeOut) where T : class
        {
            // Single exception type with condition
            return await HandleSqlCall(connectionString, async (cn) =>
            {
                return await cn.QueryFirstOrDefaultAsync<T>($"[dbo].{procedureName}",
                            param: paramList,
                            commandType: CommandType.StoredProcedure,
                            commandTimeout: timeOut);
            });
        }

        /// <summary>
        /// Execute a stored procedure and get a return object
        /// </summary>
        /// <param name="procedureName">name of the procedure</param>
        /// <param name="paramList">dynamic typed list of parameters</param>
        /// <returns></returns>
        public async Task<T?> ScalarStoredProcedureAsync<T>(string procedureName, object paramList,
            string connectionString)
        {
            // Single exception type with condition
            return await HandleSqlCall(connectionString, async (cn) =>
            {
                return await cn.ExecuteScalarAsync<T>($"[dbo].{procedureName}",
                    param: paramList,
                    commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            });
        }

        /// <summary>
        /// Execute a sql and get a return object
        /// </summary>
        /// <param name="procedureName">name of the procedure</param>
        /// <param name="paramList">dynamic typed list of parameters</param>
        /// <returns></returns>
        public async Task<T?> ScalarSqlAsync<T>(string sql, object paramList,
            string connectionString)
        {
            // Single exception type with condition
            return await HandleSqlCall(connectionString, async (cn) =>
            {
                return await cn.ExecuteScalarAsync<T>(sql,
                    param: paramList,
                    commandType: CommandType.Text).ConfigureAwait(false);
            });
        }

        /// <summary>
        /// Execute a stored procedure and get a return object
        /// </summary>
        /// <param name="procedureName">name of the procedure</param>
        /// <param name="paramList">dynamic typed list of parameters</param>
        /// <returns></returns>
        public async Task<Object?> ScalarGetObjectAsync<Object>(string procedureName, object paramList,
            string connectionString)
        {
            // Single exception type with condition
            return await HandleSqlCall(connectionString, async (cn) =>
            {
                return await cn.ExecuteScalarAsync<Object>($"[dbo].{procedureName}",
                    param: paramList,
                    commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            });
        }

        /// <summary>
        /// Execute a stored procedure
        /// </summary>
        /// <param name="procedureName">name of the procedure</param>
        /// <param name="paramList">dynamic typed list of parameters</param>
        /// <returns></returns>
        public async Task<int> ExecuteStoredProcedureAsync(string procedureName, object paramList,
            string connectionString, int timeOut = 30)
        {
            // Single exception type with condition
            return await HandleSqlCall(connectionString, async (cn) =>
            {
                int result = await cn.ExecuteAsync($"[dbo].{procedureName}",
                                      param: paramList,
                                      commandType: CommandType.StoredProcedure,
                                      commandTimeout: timeOut);

                return result;
            });
        }

        /// <summary>
        /// Execute a stored procedure
        /// </summary>
        /// <param name="procedureName">name of the procedure</param>
        /// <param name="paramList">dynamic typed list of parameters</param>
        /// <returns></returns>
        public async Task<T> ExecuteStoredProcedureAndGetOutputValueAsync<T>(string procedureName, DynamicParameters paramList, string outputParameterName,
            string connectionString)
        {
            // Single exception type with condition
            return await Policy
              .Handle<SqliteException>(ex => ex.ErrorCode == 1205) // sql deadlock resource (https://sqlbak.com/academy/transaction-process-id-was-deadlocked-on-lock-resources-with-another-process-and-has-been-chosen-as-the-deadlock-victim-msg-1205)
              .RetryAsync(3)
              .ExecuteAsync(async () =>
              {
                  using (SqliteConnection cn = new SqliteConnection(connectionString))
                  {
                      await cn.OpenAsync().ConfigureAwait(false);

                      await cn.ExecuteAsync($"[dbo].{procedureName}",
                          param: paramList,
                          commandType: CommandType.StoredProcedure).ConfigureAwait(false);
                      return paramList.Get<T>(outputParameterName);
                  }
              }).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute a sql statement and get a return object
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="paramList">dynamic typed list of parameters</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> QuerySqlAsync<T>(string sql, object paramList,
            string connectionString) where T : class
        {
            // Single exception type with condition
            return await HandleSqlCall(connectionString, async (cn) =>
            {
                return await cn.QueryAsync<T>(sql,
                    param: paramList,
                    commandType: CommandType.Text);
            });
        }

        /// <summary>
        /// Execute a sql statement and get a return object
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="paramList">dynamic typed list of parameters</param>
        /// <returns></returns>
        public IEnumerable<T> QuerySql<T>(string sql, object paramList,
            string connectionString) where T : class
        {
            using (IDbConnection cn = new SqliteConnection(connectionString))
            {
                return cn.Query<T>(sql,
                    param: paramList,
                    commandType: CommandType.Text);
            }
        }

        #endregion Public Methods




    }
}

