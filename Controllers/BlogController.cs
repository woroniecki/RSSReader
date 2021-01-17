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
using static RSSReader.Data.BlogRepository;
using RSSReader.Models;
using RSSReader.Data;
using RSSReader.Helpers;
using RSSReader.Dtos;

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
            //TODO return DTO
            return new ApiResponse(MsgSucceed, result_list, Status200OK);
        }

        [HttpPost("{blogId}/readpost")]
        public async Task<ApiResponse> ReadPost(int blogId, [FromBody]DataForReadPostDto data)
        {
            ApiUser user = await _userRepo.Get(BY_USERID(this.GetCurUserId()));
            if (user == null)
                return ErrUnauhtorized;

            Blog blog = await _blogRepo.Get(BY_BLOGID(blogId));
            if (blog == null)
                return ErrEntityNotExists;

            UserPostData user_post_data = null;
            var post = await _blogRepo.GetPostByUrl(data.PostUrl);
            if(post == null)
            {
                post = new Post()
                {
                    Url = data.PostUrl,
                    Blog = blog
                };
            }
            else
            {

            }

            if(user_post_data == null)
            {
                user_post_data = new UserPostData()
                {
                    Post = post
                };
            }

            //TODO return DTO
            return new ApiResponse(MsgCreated, user_post_data, Status201Created);
        }
    }
}
