using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace softblocks.Models
{
    public class LoginViewModel
    {
        public bool InvalidUser { get; set; }
        public bool IsPending { get; set; }
        public string code { get; set; }
    }
}