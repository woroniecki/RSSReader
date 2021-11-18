using System.Linq;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using Dtos.Subscriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSSReader.Helpers;
using ServiceLayer._Command;
using ServiceLayer.SubscriptionCommands;
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
        private ICommandsBus _commandBus;

        public SubscriptionController(ICommandsBus commandBus)
        {
            _commandBus = commandBus;
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
            await _commandBus.Send(new DisableSubCommand() { 
                UserId = this.GetCurUserId(),
                SubId = id
            }); 

            return new ApiResponse(MsgSucceed, null, Status200OK);

            //var result = await service.Unsubscribe(id, this.GetCurUserId());

            //if (service.Errors.Any())
            //    return new ApiResponse(service.Errors.First().ErrorMessage, null, Status400BadRequest);

            //return new ApiResponse(MsgSucceed, result, Status200OK);
        }

        [HttpPatch("{subid}/set_group/{groupid}")]
        public async Task<ApiResponse> SetGroup(int subId, int groupId, [FromServices] ISubscriptionSetGroupService service)
        {
            var result = await service.SetGroup(subId, groupId, this.GetCurUserId());

            if (service.Errors.Any())
                return new ApiResponse(service.Errors.First().ErrorMessage, null, Status400BadRequest);

            return new ApiResponse(MsgSucceed, result, Status200OK);
        }

        [HttpPatch("{subid}/update")]
        public async Task<ApiResponse> Update(int subId, [FromBody]UpdateSubscriptionRequestDto updateDto, [FromServices] IUpdateSubscriptionService service)
        {
            var result = await service.Update(subId, this.GetCurUserId(), updateDto);

            if (service.Errors.Any())
                return new ApiResponse(service.Errors.First().ErrorMessage, null, Status400BadRequest);

            return new ApiResponse(MsgSucceed, result, Status200OK);
        }


    }
}
