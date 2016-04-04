using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HaikuSystem.Models
{
    public class HttpErrorJSON
    {
        public string Message { get; set; }
        public Dictionary<string,List<string>> ModelState { get; set; }
    }
}