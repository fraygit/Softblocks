using softblocks.data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace softblocks.Models
{
    public class ResEditUserProfile
    {
        public User User { get; set; }
        public List<UserAttribute> Attributes { get; set; }
        public List<AttributeValue> Values { get; set; }
    }
}