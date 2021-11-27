using System;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

using DataLayer.Models;
using LogicLayer._GenericActions;
using Dtos.Auth;
using DbAccess.Core;

using LogicLayer.Helpers;
using LogicLayer._const;

namespace ServiceLayer.AuthServices
{
    public class AuthService : IAuthService
    {
        private IConfiguration _config;

        public AuthService(
            IConfiguration config
            )
        {
            _config = config;
        }

        public AuthTokensDto GenerateAuthTokens(ApiUser user)
        {
            var key = _config.GetSection("AppSettings:Token").Value;

            var authToken = CreateAuthToken(
                            user.Id,
                            user.UserName,
                            key,
                            out DateTime expiresTime
                            );

            var refreshToken = CreateRefreshToken(authToken);

            user.RefreshTokens.Add(refreshToken);

            return new AuthTokensDto()
            {
                AuthToken = new TokenResponseDto(authToken, expiresTime.From1970()),
                RefreshToken = new TokenResponseDto(refreshToken.Token, refreshToken.Expires.From1970())
            };
        }

        static string CreateAuthToken(string id, string name, string key, out DateTime expiresTime)
        {
            var creds = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha512Signature);

            expiresTime = DateTime.UtcNow.AddSeconds(RssConsts.AUTH_TOKEN_EXPIRES_TIME_S);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]{
                                new Claim(ClaimTypes.NameIdentifier, id),
                                new Claim(ClaimTypes.Name, name)
                    }
                ),
                //Issuer = "http://localhost:5000/",//TODO
                //Audience = "http://localhost:5000/",//TODO
                Expires = expiresTime,
                SigningCredentials = creds,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        static RefreshToken CreateRefreshToken(string authToken)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                var refreshToken = new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    AuthToken = authToken,
                    Expires = DateTime.UtcNow.AddSeconds(RssConsts.REFRESH_TOKEN_EXPIRES_TIME_S),
                    Created = DateTime.UtcNow
                };

                return refreshToken;
            }
        }
    }

    public interface IAuthService
    {
        public AuthTokensDto GenerateAuthTokens(ApiUser user);
    }
}
