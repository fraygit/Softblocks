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
    [Authorize]
    public class AppModuleController : Controller
    {
        private IAppModuleRepository _appModuleRepository;
        private IUserRepository _userRepository;

        public AppModuleController(IAppModuleRepository _appModuleRepository, IUserRepository _userRepository)
        {
            this._appModuleRepository = _appModuleRepository;
            this._userRepository = _userRepository;
        }

        // GET: AppModule
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public async Task<ActionResult> ModuleList()
        {
            var userService = new UserService(_userRepository);
            var user = await userService.Get(User.Identity.Name);
            if (!string.IsNullOrEmpty(user.CurrentOrganisation))
            {
                var modules = await _appModuleRepository.ListAll();
                return View(modules);
            }
            return View();
        }

        public async Task<ActionResult> PageList(string moduleId)
        {
            var module = await _appModuleRepository.Get(moduleId);
            return View(module);
        }

        public async Task<ActionResult> ModuleDetails(string moduleId)
        {
            var module = await _appModuleRepository.Get(moduleId);
            return View(module);
        }

        [HttpPost]
        public async Task<JsonResult> AddPage(ReqAddPage req)
        {
            try
            {
                var module = await _appModuleRepository.Get(req.ModuleId);
                if (module != null)
                {
                    req.Page.PageId = Guid.NewGuid().ToString();
                    if (module.Pages == null)
                    {
                        module.Pages = new List<AppModulePage>();
                    }

                    module.Pages.Add(req.Page);
                    await _appModuleRepository.Update(module.Id.ToString(), module);

                    var result = new JsonGenericResult
                    {
                        IsSuccess = true
                    };
                    return Json(result);
                }
                var ErrorResult = new JsonGenericResult
                {
                    IsSuccess = false,
                    Message = "Module cannot be found."
                };
                return Json(ErrorResult);
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

        [HttpPost]
        public async Task<JsonResult> Create(AppModule model)
        {
            try
            {
                var userService = new UserService(_userRepository);
                var user = await userService.Get(User.Identity.Name);
                if (!string.IsNullOrEmpty(user.CurrentOrganisation))
                {
                    var appModuleService = new AppModuleService(_appModuleRepository);
                    model.OrganisationId = user.CurrentOrganisation;
                    await appModuleService.Create(model);
                    var result = new JsonGenericResult
                    {
                        IsSuccess = true
                    };
                    return Json(result);
                }
                var ErrorResult = new JsonGenericResult
                {
                    IsSuccess = false,
                    Message = "No current organisation. Please login into one."
                };
                return Json(ErrorResult);
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