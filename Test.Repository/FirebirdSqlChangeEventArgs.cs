using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Repository
{
    public class FirebirdSqlChangeEventArgs : System.EventArgs
    {
        #region Public Properties

        public FbError ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        #endregion Public Properties

        #region Public Constructors + Destructors

        public FirebirdSqlChangeEventArgs(FbError errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }



        #endregion Public Constructors + Destructors
    }
}
