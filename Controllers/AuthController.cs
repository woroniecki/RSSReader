using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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
    public class AuthController : Controller
    {
        private UserManager<ApiUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        //private readonly IEmailSender _emailSender; UnComment if you want to add Email Verification also.

        public AuthController(UserManager<ApiUser> userManager, IConfiguration config, IMapper mapper)
        {
            _userManager = userManager;
            _config = config;
            this._mapper = mapper;
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

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]{
                                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                                new Claim(ClaimTypes.Name, user.Email)
                    }
                ),
                //Issuer = "http://localhost:5000/",//TODO
                //Audience = "http://localhost:5000/",//TODO
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = creds,
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var userToReturn = _mapper.Map<UserForReturnDto>(user);

            return new ApiResponse(
                MsgSucceed,
                new
                {
                    token = tokenHandler.WriteToken(token),
                    expiration = token.ValidTo,
                    user = userToReturn
                },
                Status200OK);
        }
    }
}
