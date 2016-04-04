using System.ComponentModel.DataAnnotations;

namespace HaikuSystem.DTO
{
    public class UserCreateDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Publish code must be numeric")]
        public string PublishCode { get; set; }
    }
}