using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Test.Models.Models;


using Test.Models.Interfaces;
using TestApi.Type;

namespace TestApi.Builders
{
    public class TokenUtils
    {
        private readonly IConfiguration _config;
        //private readonly IUserRepository _userRepository;
        private readonly int _tokenValidityInMinutes;
        public TokenUtils(IConfiguration config) 
        {
            _config = config;
            //_userRepository = userRepository;
            _tokenValidityInMinutes = int.Parse(_config["Jwt:TokenValidityInMinutes"]);
            _tokenValidityInMinutes = _tokenValidityInMinutes == 0 ? 1 : _tokenValidityInMinutes;
        }

        public string GenerateAccessToken(IUser? user)
        {
            var secret = _config["Jwt:Key"];
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer= _config["Jwt:ValidIssuer"],
                Audience= _config["Jwt:ValidAudience"],
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Role,user.Role),
                new Claim(ClaimTypes.Name, user.Name),
                }),
                Expires = DateTime.UtcNow.AddMinutes(_tokenValidityInMinutes), // Token expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken(int userId)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return userId.ToString()+":"+ Convert.ToBase64String(randomNumber);
        }

        public string? GenerateAccessTokenFromRefreshToken(RefreshToken? refreshToken, AuthResponse authResponse)
        {
            string secret= _config["Jwt:Key"];
            ClaimsPrincipal claimsPrincipal = GetPrincipalFromExpiredToken(authResponse.AccessToken);
            var _username = claimsPrincipal.Identity.Name;
            if (refreshToken.TokenValue != authResponse.RefreshToken || refreshToken.TokenExpiryTime < DateTime.UtcNow)
                return null;
            //if (claimsPrincipal.Identities.FirstOrDefault().Name=="chaminda")

            // Implement logic to generate a new access token from the refresh token
            // Verify the refresh token and extract necessary information (e.g., user ID)
            // Then generate a new access token

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject= claimsPrincipal.Identities.FirstOrDefault(),
                Expires = DateTime.UtcNow.AddMinutes(_tokenValidityInMinutes), // Extend expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;

        }

    }
}
