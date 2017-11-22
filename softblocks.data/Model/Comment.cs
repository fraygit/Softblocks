using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Model
{
    public class Comment
    {
        public ObjectId Id { get; set; }
        public ObjectId UserId { get; set; }
        public string Content { get; set; }
        public DateTime DateReplied { get; set; }
    }
}
