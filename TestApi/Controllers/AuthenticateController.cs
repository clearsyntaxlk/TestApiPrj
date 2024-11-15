using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

//using Test.DataAccess.Interfaces;
//using Test.DataAccess.Repositories;


//using Test.Sqlite.DataAccess.Repositories;

using Test.Models.Interfaces;
using Test.Models.Models;
using TestApi.Builders;
using TestApi.Type;
using TestApi.Type.BodyRequest;
using Test.Repository.Repositories;
//using Test.DataAccess.Services.MSSQL;
//using Test.DataAccess.Services.Sqlite;
using Test.DataAccess.Services.FireBird;

using Test.Mssql.DataAccess.Interfaces;

/*
Refer this 
https://www.c-sharpcorner.com/article/jwt-authentication-with-refresh-tokens-in-net-6-0/
https://www.c-sharpcorner.com/article/implementing-jwt-refresh-tokens-in-net-8-0/
*/

namespace TestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private IConfiguration _config;
        private readonly IUserService _userRepository;
        private TokenUtils _tokenUtils;
        public AuthenticateController(IConfiguration config) 
        {
            _config = config;

            //_userRepository = new UserService(_config["ConnectionStrings:msSqlConnString"]);
            //_userRepository = new UserService(_config["ConnectionStrings:sqliteConnString"]);
            _userRepository = new UserService(_config["ConnectionStrings:fireBirdConnString"]);
            
            _tokenUtils = new TokenUtils(_config);  
        }
        
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LogIn([FromBody] LogInRequest? loginRequest)
        {

            // hardcoded for testing
            loginRequest.UserName = "chaminda";
            loginRequest.Password = "password";
            // end hardcoded
            if (loginRequest is null)
                return BadRequest("Invalid client request.");

            //your logic for login process
            //- if it is authonticated then get the user details
            var _user =await _userRepository.Authenticate(loginRequest.UserName, loginRequest.Password); //   new User { Name="Chaminda",Role="admin"};
            if (_user == null)
                return Unauthorized();

            //If login usrename and password are correct then proceed to generate token
            var accessToken = _tokenUtils.GenerateAccessToken(_user);
            var refreshToken = _tokenUtils.GenerateRefreshToken(_user.Id);
            _user.RefreshToken = refreshToken;
            var refreshTokenValidityInDays = int.Parse(_config["Jwt:RefreshTokenValidityInDays"]);
            _user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenValidityInDays ==0?1: refreshTokenValidityInDays);
            await _userRepository.SaveRefreshToken(_user);
            var _authResponse = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            return Ok(_authResponse);
        }
        [HttpPost]
        //[Authorize(Roles = "refresh")]
        [Route("refresh-token")]
        public async Task<IActionResult> Refresh([FromBody] AuthResponse authResponse)
        {
            if (authResponse is null)
                return BadRequest("Invalid client request.");

            var _refreshToke = _userRepository.GetRefreshTokenByRefreshToken(authResponse.RefreshToken).Result;
            if (_refreshToke == null) return BadRequest("Invalid Refresh token");
            var _accessToken = _tokenUtils.GenerateAccessTokenFromRefreshToken(_refreshToke, authResponse);
            if (_accessToken == null) return BadRequest("Expired Refresh token or Access token");
            var _authResponse = new AuthResponse
            {
                AccessToken = _accessToken,
                RefreshToken = authResponse.RefreshToken
            };
            return Ok(_authResponse);
        }
        [Authorize]
        [HttpPost]
        [Route("revoke")]
        public async Task<IActionResult> Revoke(string username)
        {
            var _user = _userRepository.GetUserByName(username).Result;
            if (_user == null) return BadRequest("Invalid user name");

            _user.RefreshToken = null;
            await _userRepository.SaveRefreshToken(_user);

            return NoContent();
        }

        [Authorize]
        [HttpPost]
        [Route("revoke-all")]
        public async Task<IActionResult> RevokeAll()
        {
            var users = _userRepository.GetUsers().Result;
            foreach (var user in users)
            {
                user.RefreshToken = null;
                await _userRepository.SaveRefreshToken(user);
            }
            return NoContent();
        }
        /*
        [HttpPost]
        [Authorize(Roles = "refresh")]
        public ActionResult<bool> Logout()
        {
            //Mark Token as invalid
            return true;
        }*/
    }
}
