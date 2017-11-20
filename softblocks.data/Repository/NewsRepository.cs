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
    public class NewsRepository : EntityService<News>, INewsRepository
    {
        public async Task<List<News>> Get(ObjectId organisationId)
        {
            var builder = Builders<News>.Filter;
            var filter = builder.Eq("OrganisationId", organisationId);
            var listings = await ConnectionHandler.MongoCollection.Find(filter).ToListAsync();
            return listings;
        }

        public async Task<List<News>> GetStatus(ObjectId organisationId, string status)
        {
            var builder = Builders<News>.Filter;
            var filter = builder.Eq("OrganisationId", organisationId) & builder.Eq("Status", status);
            var listings = await ConnectionHandler.MongoCollection.Find(filter).ToListAsync();
            return listings;
        }
    }
}
