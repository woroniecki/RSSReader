using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AutoWrapper.Wrappers;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static RSSReader.Data.Response;
using static RSSReader.Data.Repositories.UserRepository;
using static RSSReader.Data.Repositories.BlogRepository;
using static RSSReader.Data.Repositories.PostRepository;
using static RSSReader.Data.Repositories.UserPostDataRepository;
using RSSReader.Models;
using RSSReader.Data.Repositories;
using RSSReader.Helpers;
using RSSReader.Dtos;

namespace RSSReader.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BlogController : Controller
    {
        private readonly IReaderRepository _readerRepo;
        private readonly IBlogRepository _blogRepo;
        private readonly IPostRepository _postRepo;
        private readonly IUserPostDataRepository _userPostDataRepo;
        private readonly IUserRepository _userRepo;

        public BlogController(
            IReaderRepository readerRepo,
            IBlogRepository blogRepo,
            IPostRepository postRepo,
            IUserPostDataRepository userPostDataRepo,
            IUserRepository userRepo)
        {
            _readerRepo = readerRepo;
            _blogRepo = blogRepo;
            _postRepo = postRepo;
            _userPostDataRepo = userPostDataRepo;
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
            var post = await _postRepo.Get(BY_POSTURL(data.PostUrl));
            if(post == null)
            {
                post = new Post(data.Name, data.PostUrl, blog);
                _readerRepo.Add(post);
            }
            else
            {
                user_post_data = await _userPostDataRepo.GetWithPost(
                    BY_USERPOSTDATAPOSTANDUSER(user, post)
                    );
            }

            ApiResponse returnedResponse;
            if (user_post_data == null)
            {
                user_post_data = new UserPostData(post, user);
                _readerRepo.Add(user_post_data);
                returnedResponse = new ApiResponse(MsgCreated, user_post_data, Status201Created);
            }
            else
            {
                user_post_data.LastDateOpen = DateTime.UtcNow;
                _readerRepo.Update(user_post_data);
                returnedResponse = new ApiResponse(MsgUpdated, user_post_data, Status200OK);
            }

            if (!await _readerRepo.SaveAllAsync())
                return ErrRequestFailed;

            //TODO return DTO
            return returnedResponse;
        }
    }
}
