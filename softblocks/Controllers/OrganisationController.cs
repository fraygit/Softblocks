using softblocks.data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace softblocks.Controllers
{
    [Authorize]
    public class OrganisationController : Controller
    {
        // GET: Organisation
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Organisation model)
        {


            return View();
        }
    }
}