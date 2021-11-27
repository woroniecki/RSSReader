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

namespace ServiceLayer._CQRS.UserCommands
{
    public class LoginUserCommand : ICommand
    {
        public LoginRequestDto Data { get; set; }

        private AuthTokensDto GeneratedTokens { get; set; }
        public AuthTokensDto GetGeneratedTokens() { return GeneratedTokens; }
        public void SetGeneratedTokens(AuthTokensDto value) { GeneratedTokens = value; }
    }

    public class LoginUserCommandHandler : IHandleCommand<LoginUserCommand>
    {
        private DataContext _context;
        private UserManager<ApiUser> _userManager;
        private IAuthService _authService;

        public LoginUserCommandHandler(
            DataContext context, 
            UserManager<ApiUser> userManager,
            IAuthService authService
            )
        {
            _context = context;
            _userManager = userManager;
            _authService = authService;
        }

        public async Task Handle(LoginUserCommand command)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                var user = await GetUser(command.Data.Username);

                await CheckPassword(command, user);

                AuthTokensDto tokens = _authService.GenerateAuthTokens(user);

                command.SetGeneratedTokens(tokens);

                _context.SaveChanges();

                await tx.CommitAsync();
            }
        }

        private async Task CheckPassword(LoginUserCommand command, ApiUser user)
        {
            if (!await _userManager.CheckPasswordAsync(user, command.Data.Password))
                throw ApiExceptions.General(
                                "Wrong credentials.",
                                Status400BadRequest
                            );
        }

        private async Task<ApiUser> GetUser(string username)
        {
            var user = await GetUserWithRefreshTokens(x => x.UserName == username);

            if (user == null)
                user = await GetUserWithRefreshTokens(x => x.Email == username);

            if (user == null)
                throw ApiExceptions.General(
                            "Wrong credentials.",
                            Status400BadRequest
                        );

            return user;
        }

        private async Task<ApiUser> GetUserWithRefreshTokens(Expression<Func<ApiUser, bool>> predicate)
        {
            return await _context.Users
                .Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(predicate);
        }
    }
}
