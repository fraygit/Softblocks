using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace softblocks.Controllers
{
    public class AlertPagesController : Controller
    {
        // GET: AlertPages
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult NotAvailable()
        {
            return View();
        }
    }
}