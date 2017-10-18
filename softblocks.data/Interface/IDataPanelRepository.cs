using softblocks.data.Model;
using softblocks.data.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Interface
{
    public interface IDataPanelRepository : IEntityService<DataPanel>
    {
        Task<List<DataPanel>> GetByOrg(string organisationId);
    }
}
