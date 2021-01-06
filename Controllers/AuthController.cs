using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RSSReader.Data;
using RSSReader.Dtos;
using RSSReader.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static RSSReader.Data.Response;

namespace RSSReader.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : APIBaseController
    {
        private UserManager<ApiUser> _userManager;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        //private readonly IEmailSender _emailSender; UnComment if you want to add Email Verification also.

        public AuthController(UserManager<ApiUser> userManager, IAuthService authService, IMapper mapper) 
            : base(userManager)
        {
            _userManager = userManager;
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ApiResponse> Register([FromBody] UserForRegisterDto model)
        {
            if (await _userManager.FindByNameAsync(model.Username) != null)
                ModelState.AddModelError(nameof(UserForRegisterDto.Username), MsgErrUsernameTaken);

            if (await _userManager.FindByEmailAsync(model.Email) != null)
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
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null)
                user = await _userManager.FindByEmailAsync(model.Username);

            if (user == null)
                return ErrWrongCredentials;

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
                return ErrWrongCredentials;

            var token = _authService.CreateAuthToken(user.Id, user.UserName, 
                out DateTime expiresTime);

            var refreshToken = await _authService.CreateRefreshToken(user, token);
            if (refreshToken == null)
                return ErrRequestFailed;

            var userToReturn = _mapper.Map<UserForReturnDto>(user);
            var refreshTokenToReturn = _mapper.Map<RefreshTokenForReturnDto>(refreshToken);

            return new ApiResponse(
                MsgSucceed,
                new
                {
                    token = token,
                    expiration = expiresTime,
                    refreshToken = refreshTokenToReturn,
                    user = userToReturn
                },
                Status200OK);
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<ApiResponse> Refresh()
        {
            ApiUser user = await GetCurrentUser();
            if (user == null)
                return ErrUnauhtorized;

            string value = Request.Headers["Authorization"];

            return new ApiResponse(MsgSucceed, new { value = value }, Status200OK);
        }
    }
}
