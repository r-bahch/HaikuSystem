using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HaikuSystem.Models
{
    public class Rating
    {
        public int ID { get; set; }
        [Required]
        [DisplayName("Rating")]
        [Range(1,5, ErrorMessage = "Value must be between 1 and 5")]
        public int Value { get; set; }


        public int HaikuID { get; set; }
        [ForeignKey("HaikuID")]
        public Haiku Haiku { get; set; }
    }
}