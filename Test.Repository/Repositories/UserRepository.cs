using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Models.Interfaces;
using Test.Repository.Interfaces;
using Test.Models.Models;
using System.Data;

namespace Test.Repository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        public IDbHelper _dbHelper;

        public UserRepository(string connectionString, IDbHelper dbHelper)
        {
            _connectionString = connectionString;
            _dbHelper= dbHelper;
        }
        public async Task<IUser?> GetUserById(int id)
        {
            var _users = await GetUsers();
            return _users.Where(x => x.Id == id).FirstOrDefault();
        }
        public async Task<IUser> GetUserByName(string name)
        {
            var _users = await GetUsers();
            return _users.Where(x => x.Name.ToUpper() == name.ToUpper()).FirstOrDefault();
        }
        public async Task<IUser?> Authenticate(string uerName, string password)
        {
            var _users = await GetUsers();
            return _users.Where(x => x.Name.ToUpper() == uerName.ToUpper()).FirstOrDefault();
        }
        public async Task AddNewUser(IUser user)
        {
            //---- save it 
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into UserMst (Name,Age,MdfOn,Role,EmailId,RefreshToken,RefreshTokenExpiryTime)");
            sql.Append("values (@Name,@Age,@MdfOn,@Role,@EmailId,@RefreshToken,@RefreshTokenExpiryTime)");
            await _dbHelper.InsertRecordAsync<IUser>(_connectionString, sql.ToString(), new { user.Name, user.Age, MdfOn = DateTime.Now, user.Role, user.EmailId, user.RefreshToken, RefreshTokenExpiryTime = DateTime.Now.AddDays(1) }).ConfigureAwait(false);
        }

        public async Task SaveRefreshToken(IUser user)
        {
            //---- save it 
            StringBuilder sql = new StringBuilder();
            sql.Append("update UserMst set RefreshToken=@RefreshToken,RefreshTokenExpiryTime =@RefreshTokenExpiryTime where Name=@Name");
            await _dbHelper.InsertRecordAsync<int>(_connectionString, sql.ToString(), new { user.Name, user.RefreshToken, RefreshTokenExpiryTime = DateTime.Now.AddDays(1) }).ConfigureAwait(false);
        }
        public async Task<IUser?> GetUserByUserName(string userName)
        {
            //---- save it 
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from UserMst where Name=@Name");
            var result = await _dbHelper.QuerySqlAsync<User>(sql.ToString(), new { Name = userName }, _connectionString).ConfigureAwait(false);
            if (!result.Equals(null))
                return result.FirstOrDefault();
            return null;
        }
        public async Task<RefreshToken?> GetRefreshTokenByRefreshToken(string refreshToken)
        {
            //---- save it 
            StringBuilder sql = new StringBuilder();
            sql.Append("select RefreshToken as TokenValue,RefreshTokenExpiryTime as TokenExpiryTime from UserMst where RefreshToken=@RefreshToken");
            var result = await _dbHelper.QuerySqlAsync<RefreshToken>(sql.ToString(), new { RefreshToken = refreshToken }, _connectionString).ConfigureAwait(false);
            if (!result.Equals(null))
                return result.FirstOrDefault();
            return null;
        }

        public async Task<List<IUser>> GetUsers()
        {
            List<IUser> users = new List<IUser>();
            users.Add(new User { Id = 1, Name = "admin", Role = "admin" });
            users.Add(new User { Id = 2, Name = "Chaminda", Role = "admin" });
            users.Add(new User { Id = 3, Name = "Jude", Role = "user" });
            return users;
        }
    }
}
