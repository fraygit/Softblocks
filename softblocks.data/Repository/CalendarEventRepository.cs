using MongoDB.Bson;
using MongoDB.Driver;
using softblocks.data.Interface;
using softblocks.data.Model;
using softblocks.data.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Repository
{
    public class CalendarEventRepository : EntityService<CalendarEvent>, ICalendarEventRepository
    {
        public async Task<List<CalendarEvent>> Get(DateTime start, DateTime end, string organisationId)
        {
            var builder = Builders<CalendarEvent>.Filter;
            var filter = builder.Eq("OrganisationId", organisationId);
            var listings = await ConnectionHandler.MongoCollection.Find(filter).ToListAsync();
            return listings;
        }

        public async Task<List<CalendarEvent>> GetByUser(DateTime start, DateTime end, ObjectId userId)
        {
            var builder = Builders<CalendarEvent>.Filter;
            var filter = builder.Eq("ForeignId", userId) & builder.Gte("StartDate", start) & builder.Lte("StartDate", end);
            var listings = await ConnectionHandler.MongoCollection.Find(filter).ToListAsync();
            return listings;
        }
    }
}
