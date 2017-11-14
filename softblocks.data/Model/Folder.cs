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
        public int Parent { get; set; }
        public string Name { get; set; }
    }
}
