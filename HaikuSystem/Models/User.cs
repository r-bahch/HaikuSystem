using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HaikuSystem.Models
{
    public class User
    {
        public int ID { get; set; }
        [Required]
        public string Username { get; set; }

        [Required]
        [DisplayName("Publish code")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Publish code must be numeric")]
        public string PublishCode { get; set; }
        public bool IsVip { get; set; }

        public virtual ICollection<Haiku> Haikus { get; set; }
    }
}