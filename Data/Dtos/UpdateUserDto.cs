using System.ComponentModel.DataAnnotations;

namespace api_rest.Data.Dtos
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "The name has too many characters")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Age is required")]
        [Range(1, 120, ErrorMessage = "The age is invalid.")]
        public int Age { get; set; }
        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }
    }
}
