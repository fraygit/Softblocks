using MongoDB.Bson;
using softblocks.data.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Model
{
    public class Folder : MongoEntity
    {
        public ObjectId ForeignId { get; set; }
        public string FolderType { get; set; } // Personal, Organisation
        public ObjectId Parent { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public ObjectId CreatedBy { get; set; }
    }
}
