using DataLayer.Code;
using DataLayer.Models;
using Dtos.Auth.Register;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;
using ServiceLayer._Commons;
using Dtos.Auth.Login;
using ServiceLayer.AuthServices;
using Dtos.Auth;
using Dtos.Auth.Refresh;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Linq;

namespace ServiceLayer._CQRS.UserCommands
{
    public class RefreshTokensCommand : ICommand
    {
        public TokensRequestDto Data { get; set; }

        private AuthTokensDto GeneratedTokens { get; set; }
        public AuthTokensDto GetGeneratedTokens() { return GeneratedTokens; }
        public void SetGeneratedTokens(AuthTokensDto value) { GeneratedTokens = value; }
    }

    public class RefreshTokensCommandHandler : IHandleCommand<RefreshTokensCommand>
    {
        private DataContext _context;
        private IAuthService _authService;

        public RefreshTokensCommandHandler(
            DataContext context,
            IAuthService authService
            )
        {
            _context = context;
            _authService = authService;
        }

        public async Task Handle(RefreshTokensCommand command)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                ApiUser user = await GetUserByToken(command.Data.AuthToken);

                RefreshToken refresh_token = GetAndVerifyRefreshToken(command, user);

                refresh_token.MarkAsUsed();

                var role = await _authService.GetAndCreateRole(user);

                AuthTokensDto tokens = _authService.GenerateAuthTokens(user, role);

                command.SetGeneratedTokens(tokens);

                _context.SaveChanges();

                await tx.CommitAsync();
            }
        }

        private async Task<ApiUser> GetUserByToken(string token)
        {
            string user_id = _authService.GetUserIdFromToken(token);
            if (string.IsNullOrEmpty(user_id))
                throw ApiExceptions.General("Unauthorized.", Status400BadRequest);

            var user = await _context.Users
                .Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(x => x.Id == user_id);

            if (user == null)
                throw ApiExceptions.General("Unauthorized.", Status400BadRequest);
            return user;
        }

        private static RefreshToken GetAndVerifyRefreshToken(RefreshTokensCommand command, ApiUser user)
        {
            var refresh_token = user.RefreshTokens
                                .Where(x => x.Token == command.Data.RefreshToken)
                                .FirstOrDefault();

            if (refresh_token == null ||
                refresh_token.AuthToken != command.Data.AuthToken)
                throw ApiExceptions.General("Unauthorized.", Status400BadRequest);

            if (!refresh_token.IsActive)
                throw ApiExceptions.General("Token already used.", Status400BadRequest);

            return refresh_token;
        }
    }
}
