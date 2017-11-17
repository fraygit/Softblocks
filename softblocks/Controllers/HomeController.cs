using softblocks.data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace softblocks.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IUserRepository _userRepository;

        public HomeController(IUserRepository _userRepository)
        {
            this._userRepository = _userRepository;
        }
        // GET: Home
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var currentUser = await _userRepository.GetUser(User.Identity.Name);
            return View(currentUser);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Login");
        }
    }
}