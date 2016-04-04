using HaikuSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HaikuSystem.ViewModels
{
    public class HaikuRatingVM
    {
        public int HaikuID { get; set; }
        public IEnumerable<int> Ratings { get; set; }
        public int RatingsCount { get; }
        public double AverageRating { get; set; }
    }
}