using Test.Models.Interfaces;
using Test.Models.Models;
using Test.Repository;
using Test.Repository.Interfaces;
using Test.Repository.Repositories;
using Test.Mssql.DataAccess.Interfaces;
using System;

namespace Test.DataAccess.Services.FireBird
{
    public class UserService : IUserService
    {
        //private readonly string _connectionString;
        private readonly IUserRepository _repository;
        public UserService(string connectionString)
        {
            //_connectionString = connectionString;
            _repository = new UserRepository(connectionString, new DbHelperFireBird());
        }
        public async Task<IUser?> GetUserById(int id)
        {
            return await _repository.GetUserById(id);
        }
        public async Task<IUser> GetUserByName(string name)
        {
            return await _repository.GetUserByName(name);
        }
        public async Task<IUser?> Authenticate(string uerName, string password)
        {
            return await _repository.Authenticate(uerName, password);
        }
        public async Task AddNewUser(IUser user)
        {
            await _repository.AddNewUser(user);
        }
        public async Task SaveRefreshToken(IUser user) 
        {
            await _repository.SaveRefreshToken(user);
            
        }
        public async Task<IUser?> GetUserByUserName(string userName)
        {
            return await _repository.GetUserByUserName(userName); ;
        }
        public async Task<RefreshToken?> GetRefreshTokenByRefreshToken(string refreshToken)
        {
            return await _repository.GetRefreshTokenByRefreshToken(refreshToken);
        }
        public async Task<List<IUser>> GetUsers()
        {
            return await _repository.GetUsers();
        }
    }
}
