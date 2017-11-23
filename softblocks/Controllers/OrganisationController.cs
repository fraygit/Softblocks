using MongoDB.Bson;
using softblocks.data.Interface;
using softblocks.data.Model;
using softblocks.data.Repository;
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
    [Authorize]
    public class OrganisationController : Controller
    {
        private IOrganisationRepository _organisationRepository;
        private IUserRepository _userRepository;
        private IUserAttributeRepository _userAttributeRepository;

        public OrganisationController(IOrganisationRepository _organisationRepository, IUserRepository _userRepository, IUserAttributeRepository _userAttributeRepository)
        {
            this._organisationRepository = _organisationRepository;
            this._userRepository = _userRepository;
            this._userAttributeRepository = _userAttributeRepository;
        }

        // GET: Organisation
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [Authorize]
        public ActionResult Settings()
        {
            return View();
        }

        [Authorize]
        public async Task<ActionResult> ListUserAttributes()
        {
            var currentUser = await _userRepository.GetUser(User.Identity.Name);
            var orgId = ObjectId.Empty;
            ObjectId.TryParse(currentUser.CurrentOrganisation, out orgId);
            var userAttributes = await _userAttributeRepository.GetByOrganisation(orgId);
            return View(userAttributes);
        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> AddUserAttribute(UserAttribute req)
        {
            if (!string.IsNullOrEmpty(req.Name) && !string.IsNullOrEmpty(req.DataType))
            {
                var currentUser = await _userRepository.GetUser(User.Identity.Name);
                var orgId = ObjectId.Empty;
                ObjectId.TryParse(currentUser.CurrentOrganisation, out orgId);
                req.OrganisationId = orgId;
                await _userAttributeRepository.CreateSync(req);

                var result = new JsonGenericResult
                {
                    IsSuccess = true,
                    Result = req.Id.ToString()
                };
                return Json(result);
            }
            var resultError = new JsonGenericResult
            {
                IsSuccess = false,
                Message = "Invalid Parameters"
            };
            return Json(resultError);
        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> Create(Organisation model)
        {
            try
            {
                var organisationService = new OrganisationService(_organisationRepository, _userRepository);
                //model.Users = new List<string>();
                //model.Users.Add(User.Identity.Name);

                var currentUser = await _userRepository.GetUser(User.Identity.Name);

                model.Users = new List<OrganisationUser>();
                model.Users.Add(new OrganisationUser
                {
                    IsAdmin = true,
                    UserId = currentUser.Id
                });
                await organisationService.CreateAndSetUserDefaultOrganisation(model, User.Identity.Name);
                var result = new JsonGenericResult
                {
                    IsSuccess = true
                };
                return Json(result);
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