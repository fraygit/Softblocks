using MongoDB.Bson;
using softblocks.data.Model;
using softblocks.data.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Interface
{
    public interface ILibraryFileRepository : IEntityService<LibraryFile>
    {
        Task<List<LibraryFile>> Get(ObjectId folderId, string filename);
        Task<List<LibraryFile>> GetByFolder(ObjectId folderId);
    }
}
