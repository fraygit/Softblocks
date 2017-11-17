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
    public interface IFolderRepository : IEntityService<Folder>
    {
        Task<List<Folder>> GetByParent(ObjectId parentId);
        Task<List<Folder>> Get(ObjectId parentId, ObjectId userId, ObjectId orgId);
    }
}
