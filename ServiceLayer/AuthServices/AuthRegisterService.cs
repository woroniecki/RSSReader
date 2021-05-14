using System.ComponentModel.DataAnnotations;
using System.Collections.Immutable;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using AutoMapper;

using DbAccess.Core;
using DataLayer.Models;
using LogicLayer.Auth;

using Dtos.Auth;
using Dtos.Auth.Register;
using ServiceLayer._Commons;

namespace ServiceLayer.AuthServices
{
    public class AuthRegisterService : IAuthRegisterService
    {
        private UserManager<ApiUser> _userManager;
        private IMapper _mapper;
        private IUnitOfWork _unitOfWork;
        private RegisterAction _action;

        public IImmutableList<ValidationResult> Errors =>  _action != null ? _action.Errors : null;

        public AuthRegisterService(
            UserManager<ApiUser> userManager,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<UserResponseDto> RegisterNewUser(RegisterNewUserRequestDto inData)
        {
            _action = new RegisterAction(
                    _userManager,
                    _unitOfWork
                );

            ApiUser registered_user = await _action.ActionAsync(inData);

            if (_action.HasErrors) return null;

            UserResponseDto returned_dto = _mapper.Map<UserResponseDto>(registered_user);

            return returned_dto;
        }
    }

    public interface IAuthRegisterService : IValidatedService
    {
        Task<UserResponseDto> RegisterNewUser(RegisterNewUserRequestDto inData);
    }
}
