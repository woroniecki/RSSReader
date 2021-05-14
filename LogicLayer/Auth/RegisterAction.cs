using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using DataLayer.Models;
using DbAccess.Core;
using LogicLayer._GenericActions;

using Dtos.Auth.Register;

namespace LogicLayer.Auth
{
    public class RegisterAction :
        ActionErrors,
        IActionAsync<RegisterNewUserRequestDto, ApiUser>
    {
        private UserManager<ApiUser> _userManager;
        private IUnitOfWork _UOW;

        public RegisterAction(
            UserManager<ApiUser> userManager,
            IUnitOfWork unitOfWork
            )
        {
            _userManager = userManager;
            _UOW = unitOfWork;
        }

        public async Task<ApiUser> ActionAsync(RegisterNewUserRequestDto dto)
        {
            if(await _UOW.UserRepo.GetByUsername(dto.Username) != null)
            {
                AddError("Username is already taken.", nameof(RegisterNewUserRequestDto.Username));
                return null;
            }

            if (await _UOW.UserRepo.GetByEmail(dto.Email) != null)
            {
                AddError("Email is already taken.", nameof(RegisterNewUserRequestDto.Email));
                return null;
            }

            ApiUser registered_user = new ApiUser
            {
                UserName = dto.Username,
                Email = dto.Email,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(registered_user, dto.Password);

            if (!result.Succeeded)
            {
                AddError("Internale server error.");
                return null;
            }

            return registered_user;
        }
    }
}
