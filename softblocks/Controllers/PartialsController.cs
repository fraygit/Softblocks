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
    public class PartialsController : Controller
    {

        private IUserRepository _userRepository;
        private IOrganisationRepository _organisationRepository;
        private IAppModuleRepository _appModuleRepository;

        private NavigationViewModel _model;

        public PartialsController(IUserRepository _userRepository, IOrganisationRepository _organisationRepository, IAppModuleRepository _appModuleRepository)
        {
            this._userRepository = _userRepository;
            this._organisationRepository = _organisationRepository;
            this._appModuleRepository = _appModuleRepository;
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

        public ActionResult ListModulePages()
        {
            var userService = new UserService(_userRepository);
            var user = Task.Run(() => userService.Get(User.Identity.Name)).Result;
            var modules = new List<AppModule>();
            if (!string.IsNullOrEmpty(user.CurrentOrganisation))
            {
                modules = Task.Run(() => _appModuleRepository.ListAll()).Result;
                modules = modules.Where(n => n.OrganisationId == user.CurrentOrganisation).ToList();
            }
            return PartialView(modules);
        }
    }
}