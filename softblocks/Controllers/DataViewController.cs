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
    public class DataViewController : Controller
    {
        private IAppModuleRepository _appModuleRepository;

        public DataViewController(IAppModuleRepository _appModuleRepository)
        {
            this._appModuleRepository = _appModuleRepository;
        }

        // GET: DataView
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> List(string appId)
        {
            if (!string.IsNullOrEmpty(appId))
            {
                var appModule = await _appModuleRepository.Get(appId);
                if (appModule != null)
                {
                    if (appModule.Forms != null)
                    {
                        ViewBag.AppId = appModule.Id.ToString();
                        return View(appModule.DataViews);
                    }
                    return View(new List<DataView>());
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Create(ReqCreateDataView req)
        {
            try
            {
                if (!string.IsNullOrEmpty(req.AppModuleId))
                {
                    var appModule = await _appModuleRepository.Get(req.AppModuleId);
                    if (appModule != null)
                    {
                        if (appModule.DataViews == null)
                        {
                            appModule.DataViews = new List<DataView>();
                        }

                        ObjectId dataViewId = ObjectId.GenerateNewId();

                        var dataView = new DataView
                        {
                            Id = dataViewId,
                            Name = req.Name,
                            DataViewType = req.DataViewType,
                            Description = req.Description
                        };

                        appModule.DataViews.Add(dataView);
                        await _appModuleRepository.Update(req.AppModuleId, appModule);
                        var result = new JsonGenericResult
                        {
                            IsSuccess = true,
                            Result = dataViewId.ToString()
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