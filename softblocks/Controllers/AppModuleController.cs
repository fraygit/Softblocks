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
    public class AppModuleController : Controller
    {
        private IAppModuleRepository _appModuleRepository;

        public AppModuleController(IAppModuleRepository _appModuleRepository)
        {
            this._appModuleRepository = _appModuleRepository;
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

        [HttpPost]
        public async Task<JsonResult> Create(AppModule model)
        {
            try
            {
                var appModuleService = new AppModuleService(_appModuleRepository);
                await appModuleService.Create(model);
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