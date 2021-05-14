namespace Dtos.Auth.Refresh
{
    public class TokensRequestDto
    {
        public string AuthToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
