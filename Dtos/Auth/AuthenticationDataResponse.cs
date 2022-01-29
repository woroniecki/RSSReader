using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos.Auth
{
    public class AuthenticationDataResponse
    {
        public TokenResponseDto AuthToken { get; set; }
        public TokenResponseDto RefreshToken { get; set; }
        public UserResponseDto User { get; set; }
        public string Role { get; set; }
    }
}
