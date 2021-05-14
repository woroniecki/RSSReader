using System.ComponentModel.DataAnnotations;


namespace Dtos.Auth.Login
{
    public class LoginRequestDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
