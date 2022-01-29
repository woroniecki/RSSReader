
using AutoMapper;
using DataLayer.Code;
using DataLayer.Models;
using Dtos.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ServiceLayer._CQRS.UserQueries
{
    public class GetAuthenticationDataResponseQuery : IQuery
    {
        public Expression<Func<ApiUser, bool>> Predicate;
        public AuthTokensDto Tokens { get; set; }
    }

    public class GetAuthenticationDataResponseQueryHandler : IHandleQuery<GetAuthenticationDataResponseQuery>
    {
        private DataContext _context;
        private IMapper _mapper;
        private UserManager<ApiUser> _userManager;

        public GetAuthenticationDataResponseQueryHandler(DataContext context, IMapper mapper, UserManager<ApiUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<object> Handle(GetAuthenticationDataResponseQuery query)
        {
            var user = await _context.Users
                .Where(query.Predicate)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return new AuthenticationDataResponse()
            {
                AuthToken = query.Tokens.AuthToken,
                RefreshToken = query.Tokens.RefreshToken,
                User = _mapper.Map<UserResponseDto>(user),
                Role = await user.GetRoleAndAssignToUserIfNull(_userManager)
            };
        }
    }
}
