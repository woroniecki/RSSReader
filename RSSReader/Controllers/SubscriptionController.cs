using System;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using Dtos.Subscriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSSReader.Helpers;
using ServiceLayer._CQRS;
using ServiceLayer._CQRS.BlogQueries;
using ServiceLayer._CQRS.SubscriptionCommands;
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
        private IQueriesBus _queriesBus;

        public SubscriptionController(ICommandsBus commandBus, IQueriesBus queriesBus)
        {
            _commandBus = commandBus;
            _queriesBus = queriesBus;
        }

        [HttpPost("subscribe")]
        [Authorize(Roles = "User, Admin")]
        public async Task<ApiResponse> Subscribe(SubscribeRequestDto dto)
        {
            var command = new SubscribeCommand()
            {
                UserId = this.GetCurUserId(),
                Data = dto
            };

            try
            {
                await _commandBus.Send(command);
            } 
            catch (Exception e)
            {
                return new ApiResponse(e.Message, null, Status400BadRequest);
            }

            var response = await _queriesBus.Get(
                new GetBlogResponseDtoQuery()
                {
                    Predicate = x => x.Id == command.GetSubscribedEntityId()
                });

            return new ApiResponse(MsgSucceed, response, Status200OK);
        }

        [HttpPut("{id}/unsubscribe")]
        [Authorize(Roles = "User, Admin")]
        public async Task<ApiResponse> Unsubscribe(int id)
        {
            await _commandBus.Send(new DisableSubCommand() { 
                UserId = this.GetCurUserId(),
                SubId = id
            }); 

            return new ApiResponse(MsgSucceed, null, Status200OK);
        }

        [HttpPatch("{subid}/set_group/{groupid}")]
        [Authorize(Roles = "User, Admin")]
        public async Task<ApiResponse> SetGroup(int subId, int groupId)
        {
            await _commandBus.Send(new SetGroupSubCommand()
            {
                UserId = this.GetCurUserId(),
                SubId = subId,
                NewGroupId = groupId
            });

            var response = await _queriesBus.Get(
                new GetBlogResponseDtoQuery()
                { 
                    Predicate = x => x.Id == subId
                });

            return new ApiResponse(MsgSucceed, response, Status200OK);
        }

        [HttpPatch("{subid}/update")]
        public async Task<ApiResponse> Update(int subId, [FromBody]UpdateSubscriptionRequestDto updateDto)
        {
            await _commandBus.Send(new UpdateSubCommand()
            {
                UserId = this.GetCurUserId(),
                SubId = subId,
                UpdateData = updateDto
            });

            var response = await _queriesBus.Get(
                new GetBlogResponseDtoQuery()
                {
                    Predicate = x => x.Id == subId
                });

            return new ApiResponse(MsgSucceed, response, Status200OK);
        }
    }
}
