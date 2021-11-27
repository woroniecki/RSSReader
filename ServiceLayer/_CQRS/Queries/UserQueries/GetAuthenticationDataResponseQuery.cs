
using AutoMapper;
using DataLayer.Code;
using Dtos.Auth;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer._CQRS.UserQueries
{
    public class GetAuthenticationDataResponseQuery : IQuery
    {
        public string Username { get; set; }
        public AuthTokensDto Tokens { get; set; }
    }

    public class GetAuthenticationDataResponseQueryHandler : IHandleQuery<GetAuthenticationDataResponseQuery>
    {
        private DataContext _context;
        private IMapper _mapper;

        public GetAuthenticationDataResponseQueryHandler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<object> Handle(GetAuthenticationDataResponseQuery query)
        {
            var user = await _context.Users
                .Where(x => x.UserName == query.Username)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return new AuthenticationDataResponse()
            {
                AuthToken = query.Tokens.AuthToken,
                RefreshToken = query.Tokens.RefreshToken,
                User = _mapper.Map<UserResponseDto>(user)
            };
        }
    }
}
