using System;
using System.Linq;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using Dtos.UserPostData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSSReader.Helpers;
using ServiceLayer._CQRS;
using ServiceLayer._CQRS.PostQueries;
using ServiceLayer._CQRS.UserPostDataCommands;
using ServiceLayer.PostServices;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static RSSReader.Data.Response;

namespace RSSReader.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/blog/{blogId}/[controller]/")]
    public class PostController : Controller
    {
        private IQueriesBus _queriesBus;
        private ICommandsBus _commandBus;

        public PostController(ICommandsBus commandBus, IQueriesBus queriesBus)
        {
            _queriesBus = queriesBus;
            _commandBus = commandBus;
        }

        [HttpPatch("{postId}/update")]
        public async Task<ApiResponse> UpdateUserPostData(int postId, [FromBody] UpdateUserPostDataRequestDto inData)
        {
            return await UpdateUserPostDataValues(postId, inData);
        }

        [HttpPut("{postId}")]
        public async Task<ApiResponse> ReadPost(int postId)
        {
            UpdateUserPostDataRequestDto inData =
                new UpdateUserPostDataRequestDto() { Readed = true };

            return await UpdateUserPostDataValues(postId, inData);
        }

        private async Task<ApiResponse> UpdateUserPostDataValues(int postId, UpdateUserPostDataRequestDto inData)
        {
            try
            {
                await _commandBus.Send(new UpdateUserPostDataCommand()
                {
                    UserId = this.GetCurUserId(),
                    PostId = postId,
                    Data = inData
                });
            }
            catch (Exception e)
            {
                return new ApiResponse(e.Message, null, Status400BadRequest);
            }

            var response = await _queriesBus.Get(
                new GetPostResponseDtoQuery()
                {
                    UserId = this.GetCurUserId(),
                    PostId = postId
                });

            return new ApiResponse(MsgSucceed, response, Status200OK);
        }

        [HttpGet("list/{page}")]
        [AllowAnonymous]
        public async Task<ApiResponse> GetPosts(int blogid, int page)
        {
            var response = await _queriesBus.Get(
                new GetPostResponseDtoListQuery()
                {
                    UserId = this.GetCurUserId(),
                    BlogId = blogid
                });

            return new ApiResponse(MsgSucceed, response, Status200OK);
        }
    }
}
