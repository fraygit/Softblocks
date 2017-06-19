using softblocks.library.Services;
using softblocks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace softblocks.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index(LoginViewModel model)
        {
            return View(model);
        }
        
        public async Task<ActionResult> Validate(PageLogin loginModel)
        {
            var isUserValid = Membership.ValidateUser(loginModel.Username, loginModel.Password);
            if (isUserValid)
            {
                FormsAuthentication.SetAuthCookie(loginModel.Username, false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", new LoginViewModel { InvalidUser = true });
            }
        }
    }
}