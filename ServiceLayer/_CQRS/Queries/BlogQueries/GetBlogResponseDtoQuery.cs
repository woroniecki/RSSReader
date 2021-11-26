using AutoMapper;
using DataLayer.Code;
using DataLayer.Models;
using Dtos.Blogs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ServiceLayer._CQRS.BlogQueries
{
    public class GetBlogResponseDtoQuery : IQuery
    {
        public Expression<Func<Subscription, bool>> Predicate;
    }

    public class GetBlogResponseDtoQueryHandler : IHandleQuery<GetBlogResponseDtoQuery>
    {
        private DataContext _context;
        private IMapper _mapper;

        public GetBlogResponseDtoQueryHandler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<object> Handle(GetBlogResponseDtoQuery query)
        {
            var db_query = (from s in _context.Subscriptions
                                           .Include(x => x.Blog)
                                           .Include(x => x.Group)
                                           .AsNoTracking()
                                           .Where(query.Predicate)
                            select new
                            {
                                Subscription = s,
                                UnreadedAmount = s.Blog.Posts.Count - s.UserPostDatas.Where(x => x.Readed).Count()
                            });

            var result = await db_query.FirstAsync();
            result.Subscription.UnreadedCount = result.UnreadedAmount;

            return _mapper.Map<Subscription, BlogResponseDto>(result.Subscription);
        }
    }
}
