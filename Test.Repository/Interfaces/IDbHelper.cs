using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Test.Repository.Interfaces
{
    public interface IDbHelper
    {

        Task<T?> InsertRecordAsync<T>(
        string connectionString,
        string sql,
        object paramList,
        CancellationToken? cancellationToken = null);

        void StopDependencyWatcher(string connectionString);
        //void StartDependencyWatcher(string connectionString, string commandText, CommandType commandType, SqlChangeEventHandler sqlSubscribeError, int timeOut = 30);
        IEnumerable<T> QueryStoredProcedure<T>(string procedureName, object paramList,
            string connectionString, int timeOut = 60) where T : class;
        Task<(IEnumerable<T>, IEnumerable<T2>)> QueryMultipleRecordsetsAsync<T, T2>(string procedureName, object paramList, string connectionString, int timeOut = 30);
        Task<(IEnumerable<T>, IEnumerable<T2>, IEnumerable<T3>)> QueryMultipleRecordsetsAsync<T, T2, T3>(string procedureName, object paramList, string connectionString, int timeOut = 30);
        Task<(IEnumerable<T>, IEnumerable<T2>, IEnumerable<T3>,
            IEnumerable<T4>)> QueryMultipleRecordsetsAsync<T, T2, T3, T4>(string procedureName, object paramList, string connectionString, int timeOut = 30);
        Task<(IEnumerable<T>, IEnumerable<T2>, IEnumerable<T3>,
            IEnumerable<T4>, IEnumerable<T5>)> QueryMultipleRecordsetsAsync<T, T2, T3, T4, T5>(string procedureName, object paramList, string connectionString, int timeOut = 30);
        T Scalar<T>(string procedureName, object paramList,
            string connectionString);
        void ExecuteStoredProcedure(string procedureName, object paramList,
            string connectionString);
        Task<IEnumerable<T>> QueryStoredProcedureAsync<T>(string procedureName, object paramList,
            string connectionString, int timeOut = 30) where T : class;
        Task<T?> QueryStoredProcedureFirstAsync<T>(string procedureName, object paramList,
            string connectionString) where T : class;
        Task<T?> QueryStoredProcedureFirstAsync<T>(string procedureName, object paramList,
            string connectionString, int timeOut) where T : class;
        Task<T?> ScalarStoredProcedureAsync<T>(string procedureName, object paramList,
            string connectionString);
        Task<T?> ScalarSqlAsync<T>(string sql, object paramList,
            string connectionString);
        Task<Object?> ScalarGetObjectAsync<Object>(string procedureName, object paramList,
            string connectionString);
        Task<int> ExecuteStoredProcedureAsync(string procedureName, object paramList,
            string connectionString, int timeOut = 30);
        Task<T> ExecuteStoredProcedureAndGetOutputValueAsync<T>(string procedureName, DynamicParameters paramList, string outputParameterName,
            string connectionString);
        Task<IEnumerable<T>> QuerySqlAsync<T>(string sql, object paramList,
            string connectionString) where T : class;
        IEnumerable<T> QuerySql<T>(string sql, object paramList,
            string connectionString) where T : class;



    }
}
