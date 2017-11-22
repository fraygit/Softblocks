using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Model
{
    public class Topic
    {
        public ObjectId Id { get; set; }
        public ObjectId UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime LastActivity { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
