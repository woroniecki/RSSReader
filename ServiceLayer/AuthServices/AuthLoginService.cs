using System.ComponentModel.DataAnnotations;
using System.Collections.Immutable;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using AutoMapper;

using DbAccess.Core;
using DataLayer.Models;
using LogicLayer.Auth;
using Dtos.Auth;
using Dtos.Auth.Login;

using ServiceLayer._Runners;
using ServiceLayer._Commons;

namespace ServiceLayer.AuthServices
{
    public class AuthLoginService : IAuthLoginService
    {
        private UserManager<ApiUser> _userManager;
        private IMapper _mapper;
        private IConfiguration _config;
        private IUnitOfWork _unitOfWork;
        private VerifyLoginCredentialsAction _verifyAction;
        private GenerateAuthTokensAction _generateTokensAction;

        public IImmutableList<ValidationResult> Errors
        {
            get
            {
                if (_verifyAction != null && _verifyAction.HasErrors)
                    return _verifyAction.Errors;

                if (_generateTokensAction != null && _generateTokensAction.HasErrors)
                    return _generateTokensAction.Errors;

                if (_verifyAction != null)
                    return _verifyAction.Errors;

                if (_generateTokensAction != null)
                    return _generateTokensAction.Errors;

                return null;
            }
        }

        public AuthLoginService(
            UserManager<ApiUser> userManager,
            IMapper mapper,
            IConfiguration config,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _mapper = mapper;
            _config = config;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthenticationDataResponse> Login(LoginRequestDto inData) 
        {
            _verifyAction = new VerifyLoginCredentialsAction(
                _userManager,
                _unitOfWork
                );

            var user_result = await _verifyAction.ActionAsync(inData);

            if (_verifyAction.HasErrors)
                return null;

            _generateTokensAction = new GenerateAuthTokensAction(
                _config
                );

            var runner = new RunnerWriteDb<ApiUser, AuthTokensDto>(
                _generateTokensAction,
                _unitOfWork.Context
                );

            var result = await runner.RunActionAsync(user_result);

            if (runner.HasErrors)
                return null;

            var returned_data = new AuthenticationDataResponse()
            {
                AuthToken = result.AuthToken,
                RefreshToken = result.RefreshToken,
                User = _mapper.Map<UserResponseDto>(user_result)
            };

            return returned_data;
        }
    }

    public interface IAuthLoginService : IValidatedService
    {
        Task<AuthenticationDataResponse> Login(LoginRequestDto inData);
    }
}
