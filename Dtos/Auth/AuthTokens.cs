using System;

namespace Dtos.Auth
{
    public class AuthTokens
    {
        public TokenResponseDto AuthToken { get; set; }
        public TokenResponseDto RefreshToken { get; set; }
    }
}
