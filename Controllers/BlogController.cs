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

        [HttpPut("{blogId}/post/{postId}/readed")]
        public async Task<ApiResponse> MarkReaded(int blogId, int postId, [FromBody]bool value)
        {
            ApiUser user = await _userRepo.Get(BY_USERID(this.GetCurUserId()));
            if (user == null)
                return ErrUnauhtorized;

            UserPostData user_post_data = await
                GetUserPostDataWithPost(user, blogId, postId);

            if (user_post_data == null)
                return ErrEntityNotExists;

            Post post = user_post_data.Post;
            if (post == null)
                return ErrEntityNotExists;

            if (user_post_data.Readed == value)
                return ErrNothingToUpdateInEntity;

            user_post_data.Readed = value;

            _readerRepo.Update(user_post_data);
            PostDataForReturnDto post_dto = MergeUserPostDataAndPost(user_post_data, post);

            if (!await _readerRepo.SaveAllAsync())
                return ErrRequestFailed;

            return new ApiResponse(MsgUpdated, post_dto, Status200OK);
        }

        [HttpPut("{blogId}/post/{postId}/favourite")]
        public async Task<ApiResponse> MarkFavourite(int blogId, int postId, [FromBody] bool value)
        {
            return null;
        }

        [HttpPut("{blogId}/post/{postId}")]
        public async Task<ApiResponse> ReadPost(int blogId, int postId)
        {
            ApiUser user = await _userRepo.Get(BY_USERID(this.GetCurUserId()));
            if (user == null)
                return ErrUnauhtorized;

            UserPostData user_post_data = await 
                GetUserPostDataWithPost(
                    user,
                    blogId,
                    postId
                );

            if (user_post_data == null)
                return ErrEntityNotExists;

            Post post = user_post_data.Post;
            if (post == null)
                return ErrEntityNotExists;

            user_post_data.LastDateOpen = DateTime.UtcNow;
            user_post_data.Readed = true;
            _readerRepo.Update(user_post_data);

            PostDataForReturnDto post_dto = MergeUserPostDataAndPost(user_post_data, post);

            if (!await _readerRepo.SaveAllAsync())
                return ErrRequestFailed;

            return new ApiResponse(MsgUpdated, post_dto, Status200OK);
        }

        async Task<UserPostData> GetUserPostDataWithPost(ApiUser user, int blogId, int postId)
        {
            Blog blog = await _blogRepo.Get(BY_BLOGID(blogId));
            if (blog == null)
                return null;

            Post post = await _postRepo.Get(BY_POSTID(postId));
            if (post == null)
                return null;

            UserPostData user_post_data = await _userPostDataRepo.GetWithPost(
                    BY_USERANDPOST(user, post)
                    );
            
            if (user_post_data == null)
            {
                user_post_data = new UserPostData(post, user);
                _readerRepo.Add(user_post_data);
            }

            return user_post_data;
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

        private PostDataForReturnDto MergeUserPostDataAndPost(UserPostData user_post_data, Post post)
        {
            PostDataForReturnDto post_dto = _mapper.Map<Post, PostDataForReturnDto>(post);
            post_dto.Readed = user_post_data.Readed;
            post_dto.Favourite = user_post_data.Favourite;
            return post_dto;
        }
    }
}
