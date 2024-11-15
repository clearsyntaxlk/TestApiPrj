using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Models.Models
{
    public class RefreshToken :Token
    {
        public DateTime? TokenExpiryTime { get; set; }
    }
}
