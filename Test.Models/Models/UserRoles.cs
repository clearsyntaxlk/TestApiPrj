using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Models.Interfaces;

namespace Test.Models.Models
{
    public class UserRoles : IUserRoles
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }
}
