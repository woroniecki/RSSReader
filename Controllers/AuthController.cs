using System.Linq;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using Dtos.Auth.Login;
using Dtos.Auth.Refresh;
using Dtos.Auth.Register;
using Microsoft.AspNetCore.Mvc;
using RSSReader.Helpers;
using ServiceLayer.AuthServices;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static RSSReader.Data.Response;

namespace RSSReader.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        [HttpPost]
        [Route("register")]
        public async Task<ApiResponse> Register([FromBody] RegisterNewUserRequestDto model, [FromServices] IAuthRegisterService service)
        {
            var response = await service.RegisterNewUser(model);

            if (service.Errors.Any())
                return new ApiResponse
                    (
                        "Error",
                        ModelErrors.ConvertResponse(service.Errors),
                        Status400BadRequest
                    );

            return new ApiResponse(MsgCreatedRecord, response, Status201Created);
        }

        [HttpPost]
        [Route("login")]
        public async Task<ApiResponse> Login([FromBody] LoginRequestDto model, [FromServices] IAuthLoginService service)
        {
            var response = await service.Login(model);

            if (service.Errors.Any())
                return new ApiResponse
                    (
                        "Error",
                        ModelErrors.ConvertResponse(service.Errors),
                        Status400BadRequest
                    );

            return new ApiResponse(MsgSucceed, response, Status200OK);
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<ApiResponse> Refresh([FromBody] TokensRequestDto data, [FromServices] IAuthRefreshTokensService service)
        {
            var response = await service.RefreshTokens(data);

            if (service.Errors.Any())
                return new ApiResponse
                    (
                        "Error",
                        ModelErrors.ConvertResponse(service.Errors),
                        Status400BadRequest
                    );

            return new ApiResponse(MsgSucceed, response, Status200OK);
        }
    }
}
