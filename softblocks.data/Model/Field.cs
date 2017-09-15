using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using softblocks.data.Common;

namespace softblocks.data.Model
{
    public class Field
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DataType { get; set; }
        public List<Field> Fields { get; set; }
    }
}
