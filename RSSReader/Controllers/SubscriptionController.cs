using System.Linq;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using Dtos.Subscriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSSReader.Helpers;
using ServiceLayer.SubscriptionServices;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static RSSReader.Data.Response;

namespace RSSReader.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class SubscriptionController : Controller
    {
        [HttpGet("list")]
        public async Task<ApiResponse> GetList([FromServices] ISubscriptionListService service)
        {
            var list = await service.GetListAsync(this.GetCurUserId());
            return new ApiResponse(MsgSucceed, list, Status200OK);
        }

        [HttpPost("subscribe")]
        public async Task<ApiResponse> Subscribe(SubscribeRequestDto dto, [FromServices] ISubscribeService service)
        {
            var result = await service.Subscribe(dto, this.GetCurUserId());

            if (service.Errors.Any())
                return new ApiResponse(service.Errors.First().ErrorMessage, null, Status400BadRequest);

            return new ApiResponse(MsgSucceed, result, Status200OK);
        }

        [HttpPut("{id}/unsubscribe")]
        public async Task<ApiResponse> Unsubscribe(int id, [FromServices] IUnsubscribeService service)
        {
            var result = await service.Unsubscribe(id, this.GetCurUserId());

            if (service.Errors.Any())
                return new ApiResponse(service.Errors.First().ErrorMessage, null, Status400BadRequest);

            return new ApiResponse(MsgSucceed, result, Status200OK);
        }

        [HttpPatch("{subid}/set_group/{groupid}")]
        public async Task<ApiResponse> SetGroup(int subId, int groupId, [FromServices] ISubscriptionSetGroupService service)
        {
            var result = await service.SetGroup(subId, groupId, this.GetCurUserId());

            if (service.Errors.Any())
                return new ApiResponse(service.Errors.First().ErrorMessage, null, Status400BadRequest);

            return new ApiResponse(MsgSucceed, result, Status200OK);
        }
    }
}
