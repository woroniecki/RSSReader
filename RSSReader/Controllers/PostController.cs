using System.Linq;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using Dtos.UserPostData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSSReader.Helpers;
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
        [HttpPatch("{postId}/update")]
        public async Task<ApiResponse> UpdateUserPostData(int postId, [FromBody] UpdateUserPostDataRequestDto inData, [FromServices] IUpdateUserPostDataService service)
        {
            var result = await service.Update(inData, postId, this.GetCurUserId());

            if (service.Errors.Any())
                return new ApiResponse(service.Errors.First().ErrorMessage, null, Status400BadRequest);

            return new ApiResponse(MsgSucceed, result, Status200OK);
        }

        [HttpPut("{postId}")]
        public async Task<ApiResponse> ReadPost(int blogId, int postId, [FromServices] IUpdateUserPostDataService service)
        {
            UpdateUserPostDataRequestDto inData = 
                new UpdateUserPostDataRequestDto() { Readed = true };

            var result = await service.Update(inData, postId, this.GetCurUserId());

            if (service.Errors.Any())
                return new ApiResponse(service.Errors.First().ErrorMessage, null, Status400BadRequest);

            return new ApiResponse(MsgSucceed, result, Status200OK);
        }

        [HttpGet("list/{page}")]
        [AllowAnonymous]
        public async Task<ApiResponse> GetPosts(int blogid, int page, [FromServices] IPostListService service)
        {
            string user_id = this.IsLoggedIn() ? this.GetCurUserId() : "";

            var result = await service.GetList(user_id, blogid, page);

            if (service.Errors.Any())
                return new ApiResponse(service.Errors.First().ErrorMessage, null, Status400BadRequest);

            return new ApiResponse(MsgSucceed, result, Status200OK);
        }
    }
}
