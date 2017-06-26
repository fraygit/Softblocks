using softblocks.data.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Model
{
    public class ModuleMenu : MongoEntity
    {
        public string Name { get; set; }
        public string Title { get; set; } 
        public string Description { get; set; }
        public string Page { get; set; }
        public List<ModuleMenu> SubMenu { get; set; }
    }
}
