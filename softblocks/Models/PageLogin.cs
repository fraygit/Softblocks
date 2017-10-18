using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace softblocks.Models
{
    public class PageLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsInvalid { get; set; }
    }
}