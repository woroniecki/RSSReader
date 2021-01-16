using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AutoWrapper.Wrappers;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static RSSReader.Data.Response;
using static RSSReader.Data.UserRepository;
using RSSReader.Models;
using RSSReader.Data;
using RSSReader.Helpers;

namespace RSSReader.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BlogController : Controller
    {
        private readonly IBlogRepository _blogRepo;
        private readonly IUserRepository _userRepo;

        public BlogController(IBlogRepository blogRepo, IUserRepository userRepo)
        {
            _blogRepo = blogRepo;
            _userRepo = userRepo;
        }

        [HttpGet("{blogId}/list")]
        public async Task<ApiResponse> GetUserPostDataList(int blogId)
        {
            ApiUser user = await _userRepo.Get(BY_USERID(this.GetCurUserId()));

            if (user == null)
                return ErrUnauhtorized;

            var result_list = await _blogRepo.GetUserPostDatasAsync(blogId, user.Id);

            return new ApiResponse(MsgSucceed, result_list, Status200OK);
        }

        [HttpPost("{blogId}/readpost")]
        public async Task<ApiResponse> ReadPost()
        {
            return new ApiResponse(MsgCreated, null, Status201Created);
        }
    }
}
