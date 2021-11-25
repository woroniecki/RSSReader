using AutoMapper;
using DataLayer.Code;
using DataLayer.Models;
using Dtos.Blogs;
using Microsoft.EntityFrameworkCore;
using ServiceLayer._Queries;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ServiceLayer.BlogQueries
{
    public class GetBlogResponseDtoBySubIdQuery : IQuery
    {
        public Expression<Func<Subscription, bool>> Predicate;
    }

    public class GetBlogResponseDtoByIdQueryHandler : IHandleQuery<GetBlogResponseDtoBySubIdQuery>
    {
        private DataContext _context;
        private IMapper _mapper;

        public GetBlogResponseDtoByIdQueryHandler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<object> Handle(GetBlogResponseDtoBySubIdQuery query)
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
