using AutoMapper;
using DataLayer.Code;
using DataLayer.Models;
using Dtos.Blogs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer._CQRS.BlogQueries
{
    public class SearchBlogsQuery : IQuery
    {
        public string SearchValue { get; set; }
    }

    public class SearchBlogsQueryHandler : IHandleQuery<SearchBlogsQuery>
    {
        private DataContext _context;
        private IMapper _mapper;

        public SearchBlogsQueryHandler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<object> Handle(SearchBlogsQuery query)
        {
            var list = await _context.Blogs
                .Where(x => x.Url.Contains(query.SearchValue) || 
                            x.Name.Contains(query.SearchValue))
                .OrderBy(x => x.Name)
                .Take(5)
                .ToListAsync();

            return _mapper.Map<IEnumerable<Blog>, IEnumerable<SearchBlogResponseDto>>(list);
        }
    }
}
