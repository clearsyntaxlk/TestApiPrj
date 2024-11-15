using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSDK.Type;

namespace TestSDK.Object
{
    public class UsersDto :GenericResponse
    {
        public IEnumerable<User> users { get; set; }
    }
}
