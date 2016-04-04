using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HaikuAPI.DTO
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