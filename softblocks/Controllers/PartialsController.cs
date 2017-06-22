using softblocks.data.Interface;
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
    public class PartialsController : Controller
    {

        private IUserRepository _userRepository;
        private IOrganisationRepository _organisationRepository;

        private NavigationViewModel _model;

        public PartialsController(IUserRepository _userRepository, IOrganisationRepository _organisationRepository)
        {
            this._userRepository = _userRepository;
            this._organisationRepository = _organisationRepository;

        }

        // GET: Partials
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Navigation()
        {
            _model = new NavigationViewModel();
            var userService = new UserService(_userRepository);

            var user = Task.Run(() => userService.Get(User.Identity.Name)).Result;

            if (!string.IsNullOrEmpty(user.CurrentOrganisation))
            {
                var organisationService = new OrganisationService(_organisationRepository, _userRepository);
                var organisation = Task.Run(() => organisationService.Get(user.CurrentOrganisation)).Result;
                if (organisation != null)
                {
                    _model.OrganisationName = organisation.Name;
                }
            }
            return PartialView(_model);
        }

        public ActionResult Header()
        {
            return PartialView();
        }
    }
}