using MongoDB.Bson;
using softblocks.data.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Model
{
    public class LibraryFile : MongoEntity
    {
        public ObjectId FolderId { get; set; }
        public string Filename { get; set; }
        public string Description { get; set; }
        public string RemotePath { get; set; }
        public string FileType { get; set; } // Personal / Organisation
        public List<LibraryFileVersion> Versions { get; set; }
        public DateTime Created { get; set; }
        public ObjectId UploadedBy { get; set; }
    }
}
