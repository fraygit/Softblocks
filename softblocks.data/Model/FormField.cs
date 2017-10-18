using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Model
{
    public class FormField
    {
        public ObjectId Id { get; set; }
        public ObjectId FieldId { get; set; }
        public int Order { get; set; }
        public int ColWidth { get; set; }
    }
}
