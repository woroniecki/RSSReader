using AutoMapper;
using DataLayer.Code;
using DataLayer.Models;
using Dtos.Groups;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer._CQRS.GroupQueries
{
    public class GetGroupResponseDtoListQuery : IQuery
    {
        public string UserId { get; set; }
    }

    public class GetGroupResponseDtoListQueryHandler : IHandleQuery<GetGroupResponseDtoListQuery>
    {
        private DataContext _context;
        private IMapper _mapper;

        public GetGroupResponseDtoListQueryHandler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<object> Handle(GetGroupResponseDtoListQuery query)
        {
            var result_list = await _context.Groups
                .Where(x => x.User.Id == query.UserId)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<Group>, IEnumerable<GroupResponseDto>>(result_list);
        }
    }
}
