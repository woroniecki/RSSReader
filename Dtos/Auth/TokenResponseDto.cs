namespace Dtos.Auth
{
    public class TokenResponseDto
    {
        public TokenResponseDto(string token, long expires)
        {
            Token = token;
            Expires = expires;
        }
        public string Token { get; set; }
        public long Expires { get; set; }
    }
}
