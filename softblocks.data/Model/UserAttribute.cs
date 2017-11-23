using MongoDB.Bson;
using softblocks.data.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Model
{
    public class UserAttribute : MongoEntity
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public ObjectId OrganisationId { get; set; }
        public int Sort { get; set; }
        public int Column { get; set; }
        public bool IsFeatured { get; set; }
    }
}
