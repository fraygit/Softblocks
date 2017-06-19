using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace softblocks.Controllers
{
    public class PartialsController : Controller
    {
        // GET: Partials
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Navigation()
        {
            return PartialView();
        }

        public ActionResult Header()
        {
            return PartialView();
        }
    }
}