using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace softblocks.Controllers
{
    public class DataViewController : Controller
    {
        // GET: DataView
        public ActionResult Index()
        {
            return View();
        }
    }
}