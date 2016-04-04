using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HaikuAPI.DTO
{
    public class UserDTO
    {
        private IEnumerable<HaikuDTO> haikus;
        public string Username { get; set; }
        public double Rating
        {
            get
            {
                return Haikus.Count() == 0 ? 0 : Haikus.Average(h => h.Rating);
            }
        }
        public IEnumerable<HaikuDTO> Haikus { get; set; }
    }
}