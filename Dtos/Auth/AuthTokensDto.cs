using System;

namespace Dtos.Auth
{
    public class AuthTokensDto
    {
        public TokenResponseDto AuthToken { get; set; }
        public TokenResponseDto RefreshToken { get; set; }
    }
}
