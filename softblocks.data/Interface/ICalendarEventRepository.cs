using MongoDB.Bson;
using softblocks.data.Model;
using softblocks.data.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Interface
{
    public interface ICalendarEventRepository : IEntityService<CalendarEvent>
    {
        Task<List<CalendarEvent>> Get(DateTime start, DateTime end, string organisationId);
        Task<List<CalendarEvent>> GetByUser(DateTime start, DateTime end, ObjectId userId);
    }
}
