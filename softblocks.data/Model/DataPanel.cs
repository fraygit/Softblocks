using softblocks.data.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Model
{
    public class DataPanel : MongoEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string OrganisationId { get; set; }
        public string Type { get; set; }
    }
}
