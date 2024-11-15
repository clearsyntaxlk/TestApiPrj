using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSDK.Object;

namespace TestSDK
{
    public interface ITestInterface
    {
        Task<UsersDto> GetUsers(string token);
        Task<string> AuthenticateAsync();
    }
}
