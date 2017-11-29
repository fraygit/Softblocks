using MongoDB.Bson;
using softblocks.data.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Model
{
    public class AttributeValue : MongoEntity
    {
        public ObjectId AttributeId { get; set; }
        public ObjectId ForeignId { get; set; }
        public string Value { get; set; }
    }
}
