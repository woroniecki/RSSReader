using AutoMapper;
using DataLayer.Code;
using DataLayer.Models;
using Dtos.Groups;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ServiceLayer._CQRS.GroupQueries
{
    public class GetGroupResponseDtoQuery : IQuery
    {
        public Expression<Func<Group, bool>> Predicate;
    }

    public class GetGroupResponseDtoQueryHandler : IHandleQuery<GetGroupResponseDtoQuery>
    {
        private DataContext _context;
        private IMapper _mapper;

        public GetGroupResponseDtoQueryHandler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<object> Handle(GetGroupResponseDtoQuery query)
        {
            var result = await _context.Groups
                .Where(query.Predicate)
                .AsNoTracking()
                .FirstAsync();

            return _mapper.Map<Group, GroupResponseDto>(result);
        }
    }
}
