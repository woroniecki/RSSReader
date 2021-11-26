using AutoMapper;
using DataLayer.Code;
using Dtos.Posts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer._CQRS.PostQueries
{
    public class GetPostResponseDtoQuery : IQuery
    {
        public string UserId { get; set; }
        public int PostId { get; set; }
    }

    public class GetPostResponseDtoQueryHandler : IHandleQuery<GetPostResponseDtoQuery>
    {
        private DataContext _context;
        private IMapper _mapper;

        public GetPostResponseDtoQueryHandler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<object> Handle(GetPostResponseDtoQuery query)
        {
            var db_query = (from post in _context.Posts
                            where post.Id == query.PostId
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

            var result = await db_query.FirstAsync();

            return _mapper.Map<PostAndUserDataSelection, PostResponseDto>(result); ;
        }
    }
}
