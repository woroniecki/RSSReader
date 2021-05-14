using System.ComponentModel.DataAnnotations;

namespace Dtos.Groups
{
    public class AddGroupRequestDto
    {
        [Required(ErrorMessage = "Name required")]
        [MinLength(1, ErrorMessage = "Name can't have less than 1 character")]
        public string Name { get; set; }
    }
}
