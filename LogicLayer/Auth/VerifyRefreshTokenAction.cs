using System;
using System.Threading.Tasks;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Linq;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using DataLayer.Models;
using LogicLayer._GenericActions;
using Dtos.Auth.Refresh;
using DbAccess.Core;

namespace LogicLayer.Auth
{
    public class VerifyRefreshTokenAction :
        ActionErrors,
        IActionAsync<TokensRequestDto, ApiUser>
    {
        private IConfiguration _config;
        private IUnitOfWork _unitOfWork;

        public VerifyRefreshTokenAction(
            IConfiguration config,
            IUnitOfWork unitOfWork
            )
        {
            _config = config;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiUser> ActionAsync(TokensRequestDto dto)
        {
            string user_id = GetUserIdFromToken(dto.AuthToken);
            if (string.IsNullOrEmpty(user_id))
            {
                AddError("Unauthorized.");
                return null;
            }

            var user = await _unitOfWork.UserRepo
                    .GetWithRefreshTokens(x => x.Id == user_id);

            if (user == null)
            {
                AddError("Unauthorized.");
                return null;
            }

            var refresh_token = user.RefreshTokens
                .Where(x => x.Token == dto.RefreshToken)
                .FirstOrDefault();

            if (refresh_token == null ||
                refresh_token.AuthToken != dto.AuthToken)
            {
                AddError("Unauthorized.");
                return null;
            }

            if (!refresh_token.IsActive)
            {
                AddError("Token already used.");
                return null;
            }

            refresh_token.Revoked = DateTime.UtcNow;

            return user;
        }

        private string GetUserIdFromToken(string token)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                        .GetBytes(_config.GetSection("AppSettings:Token").Value)),
                    ValidateLifetime = false
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken securityToken;

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

                var jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                    return null;

                var id_claim = principal.FindFirst(ClaimTypes.NameIdentifier);
                return id_claim != null ? id_claim.Value : null;
            }
            catch (Exception ex)
            {
                //TODO here should be some log
                return null;
            }
        }
    }
}
