using MongoDB.Bson;
using softblocks.data.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Model
{
    public class CalendarEvent : MongoEntity
    {
        public ObjectId ForeignId { get; set; }
        public ObjectId CreateByUserId { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string EventType { get; set; }
    }
}
