using softblocks.data.Interface;
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
        private IUserRepository _userRepository;

        public LoginController(IUserRepository _userRepository)
        {
            this._userRepository = _userRepository;
        }

        // GET: Login
        public async Task<ActionResult> Index(LoginViewModel model)
        {
            if (!string.IsNullOrEmpty(model.code))
            {
                var user = await _userRepository.GetUserByVerificationCode(model.code);
                user.Status = 1;
                await _userRepository.Update(user.Id.ToString(), user);
            }
            return View(model);
        }
        
        public async Task<ActionResult> Validate(PageLogin loginModel)
        {
            ViewBag.ErrorMessage = "";
            var existingUser = await _userRepository.GetUser(loginModel.Username);
            if (existingUser != null)
            {
                if (existingUser.Status == 2)
                {
                    ViewBag.ErrorMessage = "Account not yet activated.";
                    return RedirectToAction("Index", new LoginViewModel { IsPending = true });
                }
            }

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