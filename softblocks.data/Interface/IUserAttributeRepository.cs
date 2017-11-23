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
    public interface IUserAttributeRepository : IEntityService<UserAttribute>
    {
        Task<List<UserAttribute>> GetByOrganisation(ObjectId organisationId);
    }
}
