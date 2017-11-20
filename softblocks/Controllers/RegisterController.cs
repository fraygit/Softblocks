using MongoDB.Bson;
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
        private IEmailNotificationRepository _emailNotificationRepository;

        public RegisterController(IUserRepository _userRepository, IEmailNotificationRepository _emailNotificationRepository)
        {
            this._userRepository = _userRepository;
            this._emailNotificationRepository = _emailNotificationRepository;
        }

        // GET: Register
        public ActionResult Index()
        {
            return View();
        }

        private async Task NotifyRegisteredUser(string email, string name, string code)
        {
            try
            {
                var to = new List<string>();
                to.Add(email);
                var addUserEmail = new EmailNotification
                {
                    To = to,
                    From = "system@softblocks.co.nz",
                    Subject = string.Format("Softblocks - verify your email"),
                    Message = string.Format(@"Hi {0}, <br/><br/> To activate your account please click <a href='http://app.softblocks.co.nz/login?code={1}'>here</a>. <br/><br/> Thanks you.", name, code),
                    IsHtml = true
                };
                await _emailNotificationRepository.CreateSync(addUserEmail);
            }
            catch (Exception ex)
            {

            }
        }

        [HttpPost]
        public async Task<JsonResult> AddUser(RegisterViewModel model)
        {
            try
            {
                if (model.Password == model.PasswordRepeat)
                {
                    var userService = new UserService(_userRepository);

                    var existingUser = await _userRepository.GetUser(model.Username);
                    if (existingUser != null)
                    {
                        if (existingUser.Status == 1)
                        {
                            var Existingresult = new JsonGenericResult
                            {
                                IsSuccess = false,
                                Message = "Email already registered"
                            };
                            return Json(Existingresult);
                        }
                    }

                    var verficationCode = ObjectId.GenerateNewId().ToString();
                    var user = new User
                    {
                        Email = model.Username.ToLower(), // to lower for easy search
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Password = model.Password,
                        Status = 2, // pending,
                        VerificationCode = verficationCode
                    };
                    var newUser = await userService.CreateUser(user);
                    var result = new JsonGenericResult
                    {
                        IsSuccess = true,
                        Result = ""
                    };

                    await NotifyRegisteredUser(model.Username, model.FirstName, verficationCode);

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