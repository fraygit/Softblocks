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
    public class FolderRepository : EntityService<Folder>, IFolderRepository
    {
        public async Task<List<Folder>> GetByParent(ObjectId parentId)
        {
            var builder = Builders<Folder>.Filter;
            var filter = builder.Eq("Parent", parentId);
            var listings = await ConnectionHandler.MongoCollection.Find(filter).ToListAsync();
            return listings;
        }
    }
}
