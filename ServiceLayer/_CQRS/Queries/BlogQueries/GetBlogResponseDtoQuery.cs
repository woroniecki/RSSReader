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
            var sub = await _context.Subscriptions
                .Include(x => x.User)
                .Include(x => x.Blog)
                .Where(query.Predicate)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return _mapper.Map<Subscription, BlogResponseDto>(sub);
        }
    }
}
