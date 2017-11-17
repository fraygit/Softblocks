using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Model
{
    public class OrganisationUser
    {
        public ObjectId UserId { get; set; }
        public bool IsAdmin { get; set; }
    }
}
