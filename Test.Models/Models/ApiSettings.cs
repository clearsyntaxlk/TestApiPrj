using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Models.Models
{
    public class ApiSettings
    {
        #region Public Properties

        public int Port { get; set; }
        public Certificate Certificate { get; set; }

        #endregion Public Properties
    }

    public class Certificate
    {
        #region Public Properties

        public string Subject { get; set; }
        public string ServerWildcard { get; set; }

        #endregion Public Properties
    }
}
