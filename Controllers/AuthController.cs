using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RSSReader.Dtos;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RSSReader.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;

        //private readonly IEmailSender _emailSender; UnComment if you want to add Email Verification also.

        public AuthController(UserManager<IdentityUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        [HttpGet]
        public string Get()
        {
            return "getigeti";
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Wrong data");

            var user = await _userManager.FindByNameAsync(model.Username);
            if (await _userManager.FindByNameAsync(model.Username) != null)
                return BadRequest("User with this name already exists");

            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return BadRequest("User with this email already exists");

            var new_user = new IdentityUser
            {
                UserName = model.Username,
                Email = model.Email,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(new_user, model.Password);

            if (result.Succeeded)
                return Created("route to set", new_user);

            return BadRequest("Request failed");
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserForLoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Wrong data");

            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null)
                user = await _userManager.FindByEmailAsync(model.Username);

            if (user == null)
                return Unauthorized("Wrong data");

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized("Wrong data");

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
                Expires = DateTime.Now.AddHours(3),
                SigningCredentials = creds,
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                expiration = token.ValidTo,
                user = user
            });
        }
    }
}
