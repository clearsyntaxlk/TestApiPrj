using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Models.Interfaces;
using Test.Models.Models;

namespace Test.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<IUser> GetUserById(int id);
        Task<IUser> GetUserByName(string name);
        Task AddNewUser(IUser user);
        Task<IUser> Authenticate(string uerName, string password);
        Task SaveRefreshToken(IUser user);
        Task<IUser> GetUserByUserName(string userName);
        Task<RefreshToken?> GetRefreshTokenByRefreshToken(string refreshToken);
        Task<List<IUser>> GetUsers();
    }
}
