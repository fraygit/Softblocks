using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using softblocks.data.Common;
using MongoDB.Bson;

namespace softblocks.data.Model
{
    public class Field
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string DataType { get; set; }
        public List<Field> Fields { get; set; }
    }
}
