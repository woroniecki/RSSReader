using AutoMapper;
using DataLayer.Code;
using DataLayer.Models;
using Dtos.Blogs;
using Dtos.Posts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ServiceLayer._CQRS.BlogQueries
{
    public class GetPostResponseDtoListQuery : IQuery
    {
        public string UserId { get; set; }
        public int BlogId { get; set; }
    }

    public class PostAndUserDataSelection
    {
        public Post Post { get; set; }
        public UserPostData UserPostData { get; set; }
    }

    public class GetPostResponseDtoListQueryHandler : IHandleQuery<GetPostResponseDtoListQuery>
    {
        private DataContext _context;
        private IMapper _mapper;

        public GetPostResponseDtoListQueryHandler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<object> Handle(GetPostResponseDtoListQuery query)
        {
            var db_query = (from post in _context.Posts
                            where post.BlogId == query.BlogId
                            select new PostAndUserDataSelection()
                            {
                                Post = post,
                                UserPostData = _context
                                    .UserPostDatas
                                    .Where(
                                           x => x.User.Id == query.UserId &&
                                           x.Post.Id == post.Id
                                           )
                                    .FirstOrDefault()
                            });

            var result = await db_query.ToListAsync();

            return _mapper.Map<IEnumerable<PostAndUserDataSelection>, IEnumerable<PostResponseDto>>(result); ;
        }
    }
}
