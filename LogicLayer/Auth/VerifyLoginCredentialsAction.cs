using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using DataLayer.Models;
using DbAccess.Core;
using LogicLayer._GenericActions;

using Dtos.Auth.Login;

namespace LogicLayer.Auth
{
    public class VerifyLoginCredentialsAction :
        ActionErrors,
        IActionAsync<LoginRequestDto, ApiUser>
    {
        private UserManager<ApiUser> _userManager;
        private IUnitOfWork _UOW;

        public VerifyLoginCredentialsAction(
            UserManager<ApiUser> userManager,
            IUnitOfWork unitOfWork
            )
        {
            _userManager = userManager;
            _UOW = unitOfWork;
        }

        public async Task<ApiUser> ActionAsync(LoginRequestDto dto)
        {
            var user = await _UOW.UserRepo
                .GetWithRefreshTokens(x => x.UserName == dto.Username);

            if (user == null)
                user = await _UOW.UserRepo
                    .GetWithRefreshTokens(x => x.Email == dto.Username);

            if (user == null)
            {
                AddError("Wrong credentials.");
                return null;
            }

            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                AddError("Wrong credentials.");
                return null;
            }

            return user;
        }
    }
}
