using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HaikuAPI.Models
{
    public class Haiku
    {
        public int ID { get; set; }
        [Required]
        public string Text { get; set; }
        public DateTime SubmissionDate { get; set; }

        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public User User { get; set; }

        public ICollection<Rating> Ratings { get; set; }
    }
}