using System.ComponentModel.DataAnnotations;

namespace api_rest.Data.Dtos
{
    public class ReadUserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
    }
}
