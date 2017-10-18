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
    public class DocumentTypeRepository : EntityService<DocumentType>, IDocumentTypeRepository
    {
        public async Task<List<DocumentType>> GetByOrg(string organisationId)
        {
            var builder = Builders<DocumentType>.Filter;
            var filter = builder.Eq("OrganisationId", organisationId);
            var listings = await ConnectionHandler.MongoCollection.Find(filter).ToListAsync();
            return listings;
        }
    }
}
