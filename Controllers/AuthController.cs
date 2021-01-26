using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RSSReader.Data;
using RSSReader.Dtos;
using RSSReader.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static RSSReader.Data.Response;
using static RSSReader.Data.Repositories.UserRepository;
using RSSReader.Data.Repositories;
using RSSReader.Helpers;

namespace RSSReader.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private UserManager<ApiUser> _userManager;
        private readonly IUserRepository _userRepo;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        //private readonly IEmailSender _emailSender; UnComment if you want to add Email Verification also.

        public AuthController(
            UserManager<ApiUser> userManager, 
            IUserRepository userRepository,
            IAuthService authService, 
            IMapper mapper)
        {
            _userManager = userManager;
            _userRepo = userRepository;
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ApiResponse> Register([FromBody] UserForRegisterDto model)
        {
            if (await _userRepo.Get(BY_USERNAME(model.Username)) != null)
                ModelState.AddModelError(nameof(UserForRegisterDto.Username), MsgErrUsernameTaken);

            if (await _userRepo.Get(BY_USEREMAIL(model.Email)) != null)
                ModelState.AddModelError(nameof(UserForRegisterDto.Email), MsgErrEmailTaken);

            if (!ModelState.IsValid)
                throw new ApiProblemDetailsException(ModelState);

            var new_user = new ApiUser
            {
                UserName = model.Username,
                Email = model.Email,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(new_user, model.Password);
            var userToReturn = _mapper.Map<UserForReturnDto>(new_user);

            if (result.Succeeded)
                return new ApiResponse(MsgCreatedRecord, userToReturn, Status201Created);

            return ErrRequestFailed;
        }

        [HttpPost]
        [Route("login")]
        public async Task<ApiResponse> Login([FromBody] UserForLoginDto model)
        {
            var user = await _userRepo
                .GetWithRefreshTokens(BY_USERNAME(model.Username));

            if (user == null)
                user = await _userRepo
                    .GetWithRefreshTokens(BY_USEREMAIL(model.Username));

            if (user == null)
                return ErrWrongCredentials;

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
                return ErrWrongCredentials;

            return await GenerateTokens(user);
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<ApiResponse> Refresh([FromBody] DataForRefreshTokenDto refreshTokenDto)
        {
            string user_id = _authService.GetUserIdFromToken(refreshTokenDto.AuthToken);
            if (string.IsNullOrEmpty(user_id))
                return ErrUnauhtorized;

            var user = await _userRepo.GetWithRefreshTokens(BY_USERID(user_id));
            if (user == null)
                return ErrUnauhtorized;

            var refresh_token = user.RefreshTokens
                .Where(x => x.Token == refreshTokenDto.RefreshToken)
                .FirstOrDefault();

            if (refresh_token == null)
                return ErrEntityNotExists;

            if (refresh_token.AuthToken != refreshTokenDto.AuthToken)
                return ErrUnauhtorized;

            if (!refresh_token.IsActive)
                return ErrBadRequest;

            refresh_token.Revoked = DateTime.UtcNow;
            return await GenerateTokens(user);
        }

        private async Task<ApiResponse> GenerateTokens(ApiUser user)
        {
            var token = _authService.CreateAuthToken(user.Id, user.UserName,
                            out DateTime expiresTime);

            var refreshToken = await _authService.CreateRefreshToken(user, token);
            if (refreshToken == null)
                return ErrRequestFailed;

            var refreshTokenToReturn = _mapper.Map<TokenForReturnDto>(refreshToken);
            var authTokenToReturn = new TokenForReturnDto()
            {
                Token = token,
                Expires = expiresTime.From1970()
            };
            var userToReturn = _mapper.Map<UserForReturnDto>(user);

            return new ApiResponse(
                MsgSucceed,
                new
                {
                    authToken = authTokenToReturn,
                    refreshToken = refreshTokenToReturn,
                    user = userToReturn
                },
                Status200OK);
        }
    }
}
