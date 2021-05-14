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
using Dtos.Auth.Refresh;

using ServiceLayer._Runners;
using ServiceLayer._Commons;

namespace ServiceLayer.AuthServices
{
    public class AuthRefreshTokensService : IAuthRefreshTokensService
    {
        private IMapper _mapper;
        private IConfiguration _config;
        private IUnitOfWork _unitOfWork;
        private VerifyRefreshTokenAction _verifyAction;
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

        public AuthRefreshTokensService(
            IMapper mapper,
            IConfiguration config,
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _config = config;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthenticationDataResponse> RefreshTokens(TokensRequestDto inData)
        {
            _verifyAction = new VerifyRefreshTokenAction(
                _config,
                _unitOfWork
                );

            var user_result = await _verifyAction.ActionAsync(inData);

            if (_verifyAction.HasErrors)
                return null;

            _generateTokensAction = new GenerateAuthTokensAction(
                _config
                );

            var runner = new RunnerWriteDb<ApiUser, AuthTokens>(
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

    public interface IAuthRefreshTokensService : IValidatedService
    {
        Task<AuthenticationDataResponse> RefreshTokens(TokensRequestDto inData);
    }
}
