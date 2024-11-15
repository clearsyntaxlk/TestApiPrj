using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Repository
{
    public class SqlChangeEventArgs : System.EventArgs
    {

        #region Public Properties

        public SqlNotificationInfo Info { get; set; }
        public SqlNotificationSource Source { get; set; }

        #endregion Public Properties

        #region Public Constructors + Destructors

        public SqlChangeEventArgs(SqlNotificationSource source, SqlNotificationInfo info)
        {
            Info = info;
            Source = source;
        }

        #endregion Public Constructors + Destructors
    }
}
