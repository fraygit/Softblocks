using MongoDB.Bson;
using softblocks.data.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Model
{
    public class News : MongoEntity
    {
        public ObjectId OrganisationId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public ObjectId CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public ObjectId DateModified { get; set; }
        public ObjectId ModifiedBy { get; set; }
        public int Status { get; set; } // 0 - draft, 1 - published, 2 - ARCHIVED
    }
}
