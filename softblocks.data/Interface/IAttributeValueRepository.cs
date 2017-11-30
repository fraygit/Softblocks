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
    public interface IAttributeValueRepository: IEntityService<AttributeValue>
    {
        Task<AttributeValue> Get(ObjectId attributeId, ObjectId foreignId);
        Task<List<AttributeValue>> GetByForeignId(ObjectId foreignId);
    }
}
