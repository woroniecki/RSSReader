using System.Linq;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using Dtos.Groups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSSReader.Helpers;
using ServiceLayer._CQRS;
using ServiceLayer._CQRS.GroupQueries;
using ServiceLayer.GroupServices;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static RSSReader.Data.Response;

namespace RSSReader.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class GroupController : Controller
    {
        private IQueriesBus _queriesBus;
        private ICommandsBus _commandBus;

        public GroupController(ICommandsBus commandBus, IQueriesBus queriesBus)
        {
            _queriesBus = queriesBus;
            _commandBus = commandBus;
        }

        [HttpGet("list")]
        public async Task<ApiResponse> GetList([FromServices] IGroupListService service)
        {
            var response = await _queriesBus.Get(
                new GetGroupResponseDtoListQuery()
                {
                    UserId = this.GetCurUserId()
                });

            return new ApiResponse(MsgSucceed, response, Status200OK);
        }

        [HttpPost("add")]
        public async Task<ApiResponse> Add([FromBody] AddGroupRequestDto data, [FromServices] IGroupAddService service)
        {
            var result = await service.AddNewGroup(data, this.GetCurUserId());

            if (service.Errors.Any())
                return new ApiResponse(service.Errors.First().ErrorMessage, null, Status400BadRequest);

            return new ApiResponse(MsgSucceed, result, Status201Created);
        }

        [HttpDelete("remove")]
        public async Task<ApiResponse> Remove([FromBody] RemoveGroupRequestDto data, [FromServices] IGroupRemoveService service)
        {
            await service.Remove(data, this.GetCurUserId());

            if (service.Errors.Any())
                return new ApiResponse(service.Errors.First().ErrorMessage, null, Status400BadRequest);

            return new ApiResponse(MsgSucceed, null, Status204NoContent);
        }
    }
}
