using AutoMapper;
using DataLayer.Code;
using DataLayer.Models;
using Dtos.Blogs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer._CQRS.BlogQueries
{
    public class GetSubscribedBlogsListQuery : IQuery
    {
        public string UserId { get; set; }
    }

    public class GetSubscribedBlogsListQueryHandler : IHandleQuery<GetSubscribedBlogsListQuery>
    {
        private DataContext _context;
        private IMapper _mapper;

        public GetSubscribedBlogsListQueryHandler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<object> Handle(GetSubscribedBlogsListQuery query)
        {
            var db_query = (from s in _context.Subscriptions
                                           .Include(x => x.Blog)
                                           .Include(x => x.Group)
                                           .AsNoTracking()
                         where s.UserId == query.UserId && s.Active
                         select new
                         {
                             Subscription = s,
                             UnreadedAmount = s.Blog.Posts.Count - s.UserPostDatas.Where(x => x.Readed).Count()
                         });

            var result = await db_query.ToListAsync();

            var result_list = result.Select(x => 
            { 
                x.Subscription.UnreadedCount = x.UnreadedAmount;
                return x.Subscription;
            });

            return _mapper.Map<IEnumerable<Subscription>, IEnumerable<BlogResponseDto>>(result_list);
        }
    }
}
