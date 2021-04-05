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
using AutoMapper;

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
        private readonly IFeedService _feedService;
        private readonly IHttpService _httpService;
        private readonly IMapper _mapper;

        public BlogController(
            IReaderRepository readerRepo,
            IBlogRepository blogRepo,
            IPostRepository postRepo,
            IUserPostDataRepository userPostDataRepo,
            IUserRepository userRepo,
            IFeedService feedService,
            IHttpService httpService,
            IMapper mapper
            )
        {
            _readerRepo = readerRepo;
            _blogRepo = blogRepo;
            _postRepo = postRepo;
            _userPostDataRepo = userPostDataRepo;
            _userRepo = userRepo;
            _feedService = feedService;
            _httpService = httpService;
            _mapper = mapper;
        }

        [HttpGet("{blogId}/list")]
        public async Task<ApiResponse> GetUserPostDataList(int blogId)
        {
            ApiUser user = await _userRepo.Get(BY_USERID(this.GetCurUserId()));

            if (user == null)
                return ErrUnauhtorized;

            var result_list = await _userPostDataRepo.GetListWithPosts(
                BY_BLOGIDANDUSERID(blogId, user.Id)
                );
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
                    BY_USERANDPOST(user, post)
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

        public async Task<ApiResponse> Open(int blogid, int page = 0)
        {
            var blog = await _blogRepo.Get(BY_BLOGID(blogid));
            if (blog == null)
                return ErrEntityNotExists;

            var feed = await _httpService.GetStringContent(blog.Url);
            if (feed == null)
                return ErrExternalServerIssue;

            var parsedFeed = _feedService.ParseFeed(feed);
            if (parsedFeed == null)
                return ErrParsing;

            return new ApiResponse(MsgSucceed, parsedFeed, Status200OK);
        }

        public async Task<ApiResponse> x(int blogid, int page)
        {
            ApiUser user = await _userRepo.Get(BY_USERID(this.GetCurUserId()));

            if (user == null)
                return ErrUnauhtorized;

            var blog = await _blogRepo.Get(BY_BLOGID(blogid));
            if (blog == null)
                return ErrEntityNotExists;

            const int AMOUNT_PER_PAGE = 10;
            var posts = await _postRepo.GetLatest(blog.Id, page * AMOUNT_PER_PAGE, AMOUNT_PER_PAGE);

            var user_posts_data = await _userPostDataRepo.GetListWithPosts(
                BY_BLOGIDANDUSERID(blog.Id, user.Id)
                );

            return null;
        }

        public const int POSTS_PER_CALL = 10;
        [HttpGet("{blogid}/posts/{page}")]
        public async Task<ApiResponse> GetPosts(int blogid, int page)
        {
            ApiUser user = await _userRepo.Get(BY_USERID(this.GetCurUserId()));
            if (user == null)
                return ErrUnauhtorized;

            var blog = await _blogRepo.GetWithPosts(BY_BLOGID(blogid));
            if (blog == null)
                return ErrEntityNotExists;

            await Refresh(blog);

            IEnumerable<Post> posts = await 
                _postRepo.GetLatest(blog.Id, page * POSTS_PER_CALL, POSTS_PER_CALL);
            IEnumerable<PostDataForReturnDto> post_dtos =
                _mapper.Map<IEnumerable<Post>, IEnumerable<PostDataForReturnDto>>(posts);

            IEnumerable<UserPostData> user_posts_data = await 
                _userPostDataRepo.GetListWithPosts(BY_BLOGIDANDUSERID(blog.Id, user.Id));

            foreach(var user_post_data in user_posts_data)
            {
                PostDataForReturnDto post_dto = post_dtos
                    .FirstOrDefault(x => x.Id == user_post_data.Post.Id);
                if (post_dto != null)
                {
                    post_dto.Readed = user_post_data.Readed;
                    post_dto.Favourite = user_post_data.Favourite;
                }
            }

            return new ApiResponse(MsgSucceed, post_dtos, Status200OK);
        }

        async Task Refresh(Blog blog)
        {
            if (_feedService.ShouldRefreshBlog(blog))
            {
                var new_posts = await _feedService.RefreshBlogPosts(blog);
                if(new_posts != null && new_posts.Count() > 0)
                {
                    await _readerRepo.SaveAllAsync();
                }
            }
        }
    }
}
