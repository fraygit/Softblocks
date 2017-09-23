using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Model
{
    public class DataView
    {
        public ObjectId Id { get; set; }
        public String Name { get; set; }
        public string Description { get; set; }
        public string DataViewType { get; set; }
        public Tabular Tabular { get; set; }
        
    }
}
