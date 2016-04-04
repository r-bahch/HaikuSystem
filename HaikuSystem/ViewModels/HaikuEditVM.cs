using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HaikuSystem.ViewModels
{
    public class HaikuEditVM
    {

        public Models.Haiku TheHaiku { get; set; }
        [Required]
        [DisplayName("Publish code")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Publish code must be numeric")]
        public string AuthorPublishCode { get; set; }
    }
}