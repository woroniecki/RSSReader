using AutoWrapper.Wrappers;
using Dtos.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer._CQRS;
using ServiceLayer._CQRS.Commands.Admin;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static RSSReader.Data.Response;

namespace RSSReader.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        private IQueriesBus _queriesBus;
        private ICommandsBus _commandBus;

        public AdminController(ICommandsBus commandBus, IQueriesBus queriesBus)
        {
            _queriesBus = queriesBus;
            _commandBus = commandBus;
        }

        [HttpPost]
        [Route("setuserrole")]
        public async Task<ApiResponse> Register([FromBody] SetUserRoleRequestDto model)
        {
            await _commandBus.Send(new SetUserRoleCommand()
            {
                Username = model.Username,
                Role = model.Role
            });

            return new ApiResponse(MsgSucceed, null, Status200OK);
        }
    }
}
