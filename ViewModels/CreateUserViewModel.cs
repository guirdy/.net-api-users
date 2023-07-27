using System.ComponentModel.DataAnnotations;

namespace api_rest.ViewModels
{
    public class CreateUserViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public string Gender { get; set; }
    }

    public class UpdateUserViewModel
    {
        public string? Name { get; set; }
        public int Age { get; set; }
        public string? Gender { get; set; }
    }
}
