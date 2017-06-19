using softblocks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace softblocks.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index(LoginViewModel model)
        {
            return View(model);
        }
        
        public ActionResult Validate(PageLogin loginModel)
        {
            return RedirectToAction("Index", new LoginViewModel { InvalidUser = true});
        }
    }
}