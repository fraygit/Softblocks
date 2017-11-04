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
    public class DrodownController : Controller
    {
        private IAppModuleRepository _appModuleRepository;

        public DrodownController(IAppModuleRepository _appModuleRepository)
        {
            this._appModuleRepository = _appModuleRepository;
        }

        // GET: Drodown
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> AddDropdown(ReqAddDropdown req)
        {
            try
            {
                if (!string.IsNullOrEmpty(req.AppId))
                {
                    var appModule = await _appModuleRepository.Get(req.AppId);
                    if (appModule != null)
                    {
                        if (appModule.Dropdowns == null)
                        {
                            appModule.Dropdowns = new List<Dropdown>();
                        }

                        var dropdownId = ObjectId.GenerateNewId();
                        var newDropdown = new Dropdown
                        {
                            AppModuleId = appModule.Id,
                            Id = dropdownId,
                            Name = req.Name,
                            Items = req.Items
                        };

                        appModule.Dropdowns.Add(newDropdown);

                        await _appModuleRepository.Update(req.AppId, appModule);

                        var result = new JsonGenericResult
                        {
                            IsSuccess = true,
                            Result = dropdownId.ToString()
                        };
                        return Json(result);
                    }
                }
                var ErrorResult = new JsonGenericResult
                {
                    IsSuccess = false,
                    Message = "No app selected."
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