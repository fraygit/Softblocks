using MongoDB.Bson;
using softblocks.data.Interface;
using softblocks.data.Model;
using softblocks.Models;
using softblocks.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using softblocks.Helpers;
using System.Drawing;

namespace softblocks.Controllers
{
    public class UserController : Controller
    {
        private IUserRepository _userRepository;
        private IOrganisationRepository _organisationRepository;
        private IEmailNotificationRepository _emailNotificationRepository;
        private IUserAttributeRepository _userAttributeRepository;
        private IAttributeValueRepository _attributeValueRepository;

        public UserController(IUserRepository _userRepository, IOrganisationRepository _organisationRepository, IEmailNotificationRepository _emailNotificationRepository, IUserAttributeRepository _userAttributeRepository, IAttributeValueRepository _attributeValueRepository)
        {
            this._userRepository = _userRepository;
            this._organisationRepository = _organisationRepository;
            this._emailNotificationRepository = _emailNotificationRepository;
            this._userAttributeRepository = _userAttributeRepository;
            this._attributeValueRepository = _attributeValueRepository;
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
        [HttpPost]
        public async Task<JsonResult> SaveProfileBasicDetails()
        {
            try
            {
                HttpFileCollectionBase files = Request.Files;

                var path = Server.MapPath("~/tempUpload/");
                var reqUserId = Request["UserId"].ToString();
                var reqFirstName = Request["FirstName"].ToString();
                var reqLastName = Request["LastName"].ToString();

                if (!string.IsNullOrEmpty(reqUserId))
                {
                    var user = await _userRepository.Get(reqUserId);
                    if (user != null)
                    {
                        user.FirstName = reqFirstName;
                        user.LastName = reqLastName;
                    }                    

                    if (Request.Files.Count > 0)
                    {
                        var amazon = new AmazonService();

                        for (int i = 0; i < files.Count; i++)
                        {
                            var file = files[i];
                            var tempFilePath = Path.Combine(path, file.FileName);
                            var croppedImagePath = Path.Combine(path, "cropped_" + file.FileName);
                            file.SaveAs(tempFilePath);

                            using (var image = Image.FromFile(tempFilePath))
                            {
                                using (var croppedImage = Util.ScaleImage(image, 200, 200))
                                {
                                    croppedImage.Save(croppedImagePath);
                                    
                                }
                            }

                            var s3Path = string.Format("profilephoto/{0}", Guid.NewGuid());
                            user.ProfilePhoto = s3Path;
                            user.ProfilePhotoFilename = file.FileName;

                            using (var fileIO = System.IO.File.OpenRead(croppedImagePath))
                            {
                                using (MemoryStream tempFileStream = new MemoryStream())
                                {
                                    fileIO.CopyTo(tempFileStream);
                                    amazon.S3Upload(s3Path, MimeMapping.GetMimeMapping(file.FileName), tempFileStream);
                                }
                            }
                            System.IO.File.Delete(tempFilePath);
                            System.IO.File.Delete(croppedImagePath);
                        }
                    }// files

                    await _userRepository.Update(reqUserId, user);

                    var result = new JsonGenericResult
                    {
                        IsSuccess = true,
                        Message = ""
                    };
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                var ExResult = new JsonGenericResult
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
                return Json(ExResult);
            }
            var error = new JsonGenericResult
            {
                IsSuccess = false,
                Message = "Error saving user profile."
            };
            return Json(error);
        }

        [Authorize]
        public async Task<ActionResult> Edit(string userId)
        {
            var user = await _userRepository.Get(userId);
            if (user != null)
            {
                var result = new ResEditUserProfile();
                result.User = user;

                var organisationId = ObjectId.Empty;
                ObjectId.TryParse(user.CurrentOrganisation, out organisationId);

                var userAttributes = await _userAttributeRepository.GetByOrganisation(organisationId);
                result.Attributes = userAttributes;

                var attributeValues = await _attributeValueRepository.GetByForeignId(user.Id);
                result.Values = attributeValues;

                return View(result);
            }
            return View();
        }

        [Authorize]
        public async Task<ActionResult> Profile(string userId)
        {
            var user = await _userRepository.Get(userId);
            if (user != null)
            {
                var result = new ResEditUserProfile();
                result.User = user;

                var organisationId = ObjectId.Empty;
                ObjectId.TryParse(user.CurrentOrganisation, out organisationId);

                var userAttributes = await _userAttributeRepository.GetByOrganisation(organisationId);
                result.Attributes = userAttributes;

                var attributeValues = await _attributeValueRepository.GetByForeignId(user.Id);
                result.Values = attributeValues;

                return View(result);
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
        public async Task<JsonResult> SaveUserAttributes(ReqSaveUserAttribute req)
        {
            var currentUser = await _userRepository.GetUser(User.Identity.Name);
            if (currentUser != null)
            {
                var organisationId = ObjectId.Empty;
                ObjectId.TryParse(currentUser.CurrentOrganisation, out organisationId);

                foreach (var item in req.AtrributeValues)
                {
                    var attributeId = ObjectId.Empty;
                    ObjectId.TryParse(item.Id, out attributeId);
                    var existingValue = await _attributeValueRepository.Get(attributeId, currentUser.Id);
                    if (existingValue != null)
                    {
                        existingValue.Value = item.Value;
                        await _attributeValueRepository.Update(existingValue.Id.ToString(), existingValue);
                    }
                    else
                    {
                        var newAttributeValue = new AttributeValue
                        {
                            AttributeId = attributeId,
                            ForeignId = currentUser.Id,
                            Value = item.Value
                        };
                        await _attributeValueRepository.CreateSync(newAttributeValue);
                    }
                }
                var result = new JsonGenericResult
                {
                    IsSuccess = true,
                    Result = ""
                };
                return Json(result);


            }
            var ErrorResult = new JsonGenericResult
            {
                IsSuccess = false,
                Message = "Error adding Saving."
            };
            return Json(ErrorResult);
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
                                CurrentOrganisation = organisation.Id.ToString(),
                                Status = 3 // invited
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