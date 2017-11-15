using MongoDB.Bson;
using MongoDB.Driver;
using softblocks.data.Interface;
using softblocks.data.Model;
using softblocks.data.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Repository
{
    public class LibraryFileRepository : EntityService<LibraryFile>, ILibraryFileRepository
    {
        public async Task<List<LibraryFile>> Get(ObjectId folderId, string filename)
        {
            var builder = Builders<LibraryFile>.Filter;
            var filter = builder.Eq("FolderId", folderId) & builder.Eq("Filename", filename);
            var listings = await ConnectionHandler.MongoCollection.Find(filter).ToListAsync();
            return listings;
        }

        public async Task<List<LibraryFile>> GetByFolder(ObjectId folderId)
        {
            var builder = Builders<LibraryFile>.Filter;
            var filter = builder.Eq("FolderId", folderId);
            var listings = await ConnectionHandler.MongoCollection.Find(filter).ToListAsync();
            return listings;
        }
    }
}
