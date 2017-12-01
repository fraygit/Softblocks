using MongoDB.Bson;
using softblocks.data.Interface;
using softblocks.data.Model;
using softblocks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace softblocks.Controllers
{
    public class CalendarController : Controller
    {
        private ICalendarEventRepository _calendarEventRepository;
        private IUserRepository _userRepository;

        public CalendarController(ICalendarEventRepository _calendarEventRepository, IUserRepository _userRepository)
        {
            this._calendarEventRepository = _calendarEventRepository;
            this._userRepository = _userRepository;
        }
        // GET: Calendar
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<JsonResult> Delete(string id)
        {
            await _calendarEventRepository.Delete(id);
            var result = new JsonGenericResult
            {
                IsSuccess = true,
                Result = ""
            };
            return Json(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<JsonResult> Details(string eventId)
        {
            var eventDetails = await _calendarEventRepository.Get(eventId);
            if (eventDetails != null)
            {
                var result = new JsonGenericResult
                {
                    IsSuccess = true,
                    Result = eventDetails
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            var ErrorResult = new JsonGenericResult
            {
                IsSuccess = false,
                Message = "Error."
            };
            return Json(ErrorResult);
        }

        [Authorize]
        [HttpGet]
        public async Task<JsonResult> Events(DateTime start, DateTime end)
        {
            var events = new List<ResEvent>();

            var user = await _userRepository.GetUser(User.Identity.Name);
            if (user != null)
            {
                var eventsCalendar = await _calendarEventRepository.GetByUser(start, end, user.Id);
                foreach (var ev in eventsCalendar)
                {
                    events.Add(new ResEvent
                    {
                        id = ev.Id.ToString(),
                        title = ev.Title,
                        start = ev.StartDate,
                        end = ev.EndDate,
                        color = "#62cb31"
                    });
                }

                var orgId = ObjectId.Empty;
                ObjectId.TryParse(user.CurrentOrganisation, out orgId);
                eventsCalendar = await _calendarEventRepository.GetByUser(start, end, orgId);
                if (eventsCalendar != null)
                {
                    foreach (var ev in eventsCalendar)
                    {
                        events.Add(new ResEvent
                        {
                            id = ev.Id.ToString(),
                            title = ev.Title,
                            start = ev.StartDate,
                            end = ev.EndDate,
                            color = "#3498db"
                        });
                    }
                }

                return Json(events, JsonRequestBehavior.AllowGet);
            }

            events.Add(new ResEvent
            {
                title = "test",
                start = new DateTime(2017, 11, 2, 14, 23, 0)
            });
            return Json(events, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> AddEvent(ReqAddEvent req)
        {
            try
            {
                var user = await _userRepository.GetUser(User.Identity.Name);
                if (user != null)
                {
                    var calendarEvent = new CalendarEvent
                    {
                        CreateByUserId = user.Id,
                        Title = req.Event.Title,
                        Details = req.Event.Details,
                        StartDate = req.Event.StartDate,
                        EndDate = req.Event.EndDate,
                        EventType = req.Event.EventType
                    };
                    if (req.Event.EventType.ToLower() == "personal")
                    {
                        calendarEvent.ForeignId = user.Id;
                    }
                    else
                    {
                        calendarEvent.ForeignId = ObjectId.Parse(user.CurrentOrganisation);
                    }
                    await _calendarEventRepository.CreateSync(calendarEvent);
                    var result = new JsonGenericResult
                    {
                        IsSuccess = true,
                        Result = calendarEvent.Id
                    };
                    return Json(result);
                }
                var ErrorResult = new JsonGenericResult
                {
                    IsSuccess = false,
                    Message = "Error adding event."
                };
                return Json(ErrorResult);
            }
            catch (Exception ex)
            {
                return Json(new JsonGenericResult
                {
                    IsSuccess = false,
                    Message = ex.Message
                });

            }
        }
    }
}