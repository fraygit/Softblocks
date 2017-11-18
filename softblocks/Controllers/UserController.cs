using MongoDB.Bson;
using softblocks.data.Interface;
using softblocks.data.Model;
using softblocks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace softblocks.Controllers
{
    public class UserController : Controller
    {
        private IUserRepository _userRepository;
        private IOrganisationRepository _organisationRepository;
        private IEmailNotificationRepository _emailNotificationRepository;

        public UserController(IUserRepository _userRepository, IOrganisationRepository _organisationRepository, IEmailNotificationRepository _emailNotificationRepository)
        {
            this._userRepository = _userRepository;
            this._organisationRepository = _organisationRepository;
            this._emailNotificationRepository = _emailNotificationRepository;
        }

        // GET: User
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var currentUser = await _userRepository.GetUser(User.Identity.Name);
            if (currentUser != null)
            {
                var organisation = await _organisationRepository.Get(currentUser.CurrentOrganisation);
                ViewBag.CurrentOrganisationId = currentUser.CurrentOrganisation;
            }
            return View();
        }

        [Authorize]
        public async Task<ActionResult> ListUsers()
        {
            var currentUser = await _userRepository.GetUser(User.Identity.Name);
            if (currentUser != null)
            {
                var organisation = await _organisationRepository.Get(currentUser.CurrentOrganisation);
                ViewBag.CurrentOrganisationId = currentUser.CurrentOrganisation;
                if (organisation.Users != null)
                {
                    var orgUsers = await _userRepository.Get(organisation.Users.Select(n => n.UserId).ToArray());
                    return View(orgUsers);
                }
            }
            return View();
        }

        private async Task NotifyAddUser(string email, string name, string organisationName)
        {
            try
            {
                var to = new List<string>();
                to.Add(email);
                var addUserEmail = new EmailNotification
                {
                    To = to,
                    From = "system@softblocks.co.nz",
                    Subject = string.Format("Softblocks - You have been added to {0}", organisationName),
                    Message = string.Format(@"Hi {0}, <br/><br/> You have been added to {1}. Please login/register <a href='http://app.softblocks.co.nz'>here</a>. <br/><br/> Thanks you.", name, organisationName),
                    IsHtml = true
                };
                await _emailNotificationRepository.CreateSync(addUserEmail);
            }
            catch (Exception ex)
            {

            }
        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> AddUser(ReqAddUser req)
        {
            if (!string.IsNullOrEmpty(req.Email) && !string.IsNullOrEmpty(req.OrganisationId))
            {
                try
                {
                    ObjectId organisationId;
                    if (ObjectId.TryParse(req.OrganisationId, out organisationId))
                    {
                        var existingUser = await _userRepository.GetUser(req.Email);
                        var organisation = await _organisationRepository.Get(req.OrganisationId);
                        if (organisation.Users == null)
                        {
                            organisation.Users = new List<OrganisationUser>();
                        }

                        if (existingUser != null)
                        {
                            if (existingUser.Organisations == null)
                            {
                                existingUser.Organisations = new List<ObjectId>();
                            }

                            // add org in user table
                            if (!existingUser.Organisations.Any(n => n == organisationId))
                            {
                                existingUser.Organisations.Add(organisationId);
                                await _userRepository.Update(existingUser.Id.ToString(), existingUser);
                            }

                            // check user already existing in org
                            if (organisation.Users.Select(n => n.UserId).Any(y => y == existingUser.Id))
                            {
                                var resultExistingUser = new JsonGenericResult
                                {
                                    IsSuccess = false,
                                    Message = "User already existing!"
                                };
                                return Json(resultExistingUser);
                            }

                            // Add user in organisation
                            organisation.Users.Add(new OrganisationUser
                            {
                                IsAdmin = false,
                                UserId = existingUser.Id
                            });
                            await _organisationRepository.Update(req.OrganisationId, organisation);

                            var result = new JsonGenericResult
                            {
                                IsSuccess = true,
                                Result = existingUser.Id.ToString()
                            };
                            await NotifyAddUser(req.Email, req.FirstName, organisation.Name);
                            return Json(result);
                        }
                        else
                        {
                            var newUser = new User
                            {
                                FirstName = req.FirstName,
                                LastName = req.LastName,
                                Email = req.Email.ToLower(),
                                Organisations = new List<ObjectId>(),
                                CurrentOrganisation = organisation.Id.ToString()
                            };
                            newUser.Organisations.Add(organisationId);
                            await _userRepository.CreateSync(newUser);

                            organisation.Users.Add(new OrganisationUser
                            {
                                IsAdmin = false,
                                UserId = newUser.Id
                            });
                            await _organisationRepository.Update(req.OrganisationId, organisation);
                            await NotifyAddUser(req.Email, req.FirstName, organisation.Name);

                            var result2 = new JsonGenericResult
                            {
                                IsSuccess = true,
                                Result = newUser.Id.ToString()
                            };
                            return Json(result2);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var ErrorResultEx = new JsonGenericResult
                    {
                        IsSuccess = false,
                        Message = ex.Message
                    };
                    return Json(ErrorResultEx);
                }
            } // if
            var ErrorResult = new JsonGenericResult
            {
                IsSuccess = false,
                Message = "Error adding user."
            };
            return Json(ErrorResult);
        }
    }
}