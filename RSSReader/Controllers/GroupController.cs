using System.Linq;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using Dtos.Groups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSSReader.Helpers;
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
        [HttpGet("list")]
        public async Task<ApiResponse> GetList([FromServices] IGroupListService service)
        {
            var result = await service.GetList(this.GetCurUserId());

            if (service.Errors.Any())
                return new ApiResponse(service.Errors.First().ErrorMessage, null, Status400BadRequest);

            return new ApiResponse(MsgSucceed, result, Status200OK);
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
