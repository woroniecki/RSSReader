using System.ComponentModel.DataAnnotations;

namespace Dtos.Auth.Register
{
    public class RegisterNewUserRequestDto
    {
        [Required(ErrorMessage = "Username required")]
        [MinLength(3, ErrorMessage = "Username can't have less than 3 characters")]
        [MaxLength(20, ErrorMessage = "Username can't have more than 20 characters")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email required")]
        [EmailAddress(ErrorMessage = "Please provide correct email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password required")]
        [MinLength(3, ErrorMessage = "Password can't have less than 3 characters")]
        [MaxLength(20, ErrorMessage = "Password can't have more than 20 characters")]
        public string Password { get; set; }
    }
}
