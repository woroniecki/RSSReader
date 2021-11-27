using AutoMapper;
using DataLayer.Code;
using DataLayer.Models;
using Dtos.Auth;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer._CQRS.UserQueries
{
    public class GetUserQuery : IQuery
    {
        public Expression<Func<ApiUser, bool>> Predicate;
    }

    public class GetUserQueryHandler : IHandleQuery<GetUserQuery>
    {
        private DataContext _context;
        private IMapper _mapper;

        public GetUserQueryHandler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<object> Handle(GetUserQuery query)
        {
            var user = await _context.Users
                .Where(query.Predicate)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return _mapper.Map<UserResponseDto>(user);
        }
    }
}
