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

        public OrganisationController(IOrganisationRepository _organisationRepository, IUserRepository _userRepository)
        {
            this._organisationRepository = _organisationRepository;
            this._userRepository = _userRepository;
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

        [HttpPost]
        public async Task<JsonResult> Create(Organisation model)
        {
            try
            {
                var organisationService = new OrganisationService(_organisationRepository, _userRepository);
                model.Users = new List<string>();
                model.Users.Add(User.Identity.Name);
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