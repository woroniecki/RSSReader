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
using Microsoft.Toolkit.Parsers.Rss;
using RSSReader.Data;

namespace RSSReader.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BlogController : Controller
    {
        private readonly Data.Repositories.IReaderRepository _readerRepo;
        private readonly IBlogRepository _blogRepo;
        private readonly IPostRepository _postRepo;
        private readonly IUserPostDataRepository _userPostDataRepo;
        private readonly IUserRepository _userRepo;
        private readonly Data.IFeedService _feedService;

        public BlogController(
            Data.Repositories.IReaderRepository readerRepo,
            IBlogRepository blogRepo,
            IPostRepository postRepo,
            IUserPostDataRepository userPostDataRepo,
            IUserRepository userRepo,
            Data.IFeedService feedService)
        {
            _readerRepo = readerRepo;
            _blogRepo = blogRepo;
            _postRepo = postRepo;
            _userPostDataRepo = userPostDataRepo;
            _userRepo = userRepo;
            _feedService = feedService;
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

            ApiResponse returned_response;
            if (user_post_data == null)
            {
                user_post_data = new UserPostData(post, user);
                _readerRepo.Add(user_post_data);
                returned_response = new ApiResponse(MsgCreated, user_post_data, Status201Created);
            }
            else
            {
                user_post_data.LastDateOpen = DateTime.UtcNow;
                _readerRepo.Update(user_post_data);
                returned_response = new ApiResponse(MsgUpdated, user_post_data, Status200OK);
            }

            if (!await _readerRepo.SaveAllAsync())
                return ErrRequestFailed;

            //TODO return DTO
            return returned_response;
        }

        [HttpGet("{blogid}")]
        public async Task<ApiResponse> Open(int blogid)
        {
            var blog = await _blogRepo.Get(BY_BLOGID(blogid));
            if (blog == null)
                return ErrEntityNotExists;

            var feed = await _feedService.GetContent(blog.Url);
            if (feed == null)
                return ErrExternalServerIssue;

            var parsedFeed = _feedService.ParseFeed(feed);
            if (parsedFeed == null)
                return ErrParsing;

            return new ApiResponse(MsgSucceed, parsedFeed, Status200OK);
        }
    }
}
