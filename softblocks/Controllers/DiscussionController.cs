using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace softblocks.Controllers
{
    public class DiscussionController : Controller
    {
        // GET: Discussion
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}