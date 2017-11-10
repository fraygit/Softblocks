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
        public ActionResult Index()
        {
            return View();
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