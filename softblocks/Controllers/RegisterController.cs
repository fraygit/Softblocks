using softblocks.data.Interface;
using softblocks.data.Model;
using softblocks.library.Services;
using softblocks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace softblocks.Controllers
{
    public class RegisterController : Controller
    {
        private IUserRepository _userRepository;

        public RegisterController(IUserRepository _userRepository)
        {
            this._userRepository = _userRepository;
        }

        // GET: Register
        public ActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> AddUser(RegisterViewModel model)
        {
            try
            {
                if (model.Password == model.PasswordRepeat)
                {
                    var userService = new UserService(_userRepository);
                    var user = new User
                    {
                        Email = model.Username.ToLower(), // to lower for easy search
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Password = model.Password,
                        Status = 1 // registered
                    };
                    var newUser = await userService.CreateUser(user);
                    var result = new JsonGenericResult
                    {
                        IsSuccess = true
                    };
                    return Json(result);
                }
                return Json(new JsonGenericResult
                {
                    IsSuccess = false,
                    Message = "Password does not match."
                });
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