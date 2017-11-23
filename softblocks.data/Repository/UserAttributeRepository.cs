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
    public class UserAttributeRepository : EntityService<UserAttribute>, IUserAttributeRepository
    {
        public async Task<List<UserAttribute>> GetByOrganisation(ObjectId organisationId)
        {
            var builder = Builders<UserAttribute>.Filter;
            var filter = builder.Eq("OrganisationId", organisationId);
            var listings = await ConnectionHandler.MongoCollection.Find(filter).ToListAsync();
            return listings;
        }
    }
}
