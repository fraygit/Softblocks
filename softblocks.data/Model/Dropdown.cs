using softblocks.data.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Model
{
    public class Dropdown : MongoEntity
    {
        public string Name { get; set; }
        public string AppModuleId { get; set; }
        public List<string> Items { get; set; }
    }
}
