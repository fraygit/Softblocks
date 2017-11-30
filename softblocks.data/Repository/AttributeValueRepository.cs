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
    public class AttributeValueRepository : EntityService<AttributeValue>, IAttributeValueRepository
    {
        public async Task<AttributeValue> Get(ObjectId attributeId, ObjectId foreignId)
        {
            var builder = Builders<AttributeValue>.Filter;
            var filter = builder.Eq("AttributeId", attributeId) & builder.Eq("ForeignId", foreignId);
            var listings = await ConnectionHandler.MongoCollection.Find(filter).ToListAsync();
            if (listings.Any())
            {
                return listings.FirstOrDefault();
            }
            return null;
        }

        public async Task<List<AttributeValue>> GetByForeignId(ObjectId foreignId)
        {
            var builder = Builders<AttributeValue>.Filter;
            var filter = builder.Eq("ForeignId", foreignId);
            var listings = await ConnectionHandler.MongoCollection.Find(filter).ToListAsync();
            return listings;
        }
    }
}
