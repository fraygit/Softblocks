using MongoDB.Bson;
using MongoDB.Bson.IO;
using softblocks.data.Interface;
using softblocks.data.Model;
using softblocks.data.Service;
using softblocks.Models;
using softblocks.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace softblocks.Controllers
{
    public class DataViewController : Controller
    {
        private IAppModuleRepository _appModuleRepository;
        private IOrganisationRepository _organisationRepository;

        public DataViewController(IAppModuleRepository _appModuleRepository, IOrganisationRepository _organisationRepository)
        {
            this._appModuleRepository = _appModuleRepository;
            this._organisationRepository = _organisationRepository;
        }

        // GET: DataView
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
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

        [Authorize]
        public async Task<ActionResult> RenderTabular(string appId, string id, string dataId)
        {
            if (!string.IsNullOrEmpty(appId))
            {
                var response = new ResRenderTabular();
                var appModule = await _appModuleRepository.Get(appId);
                if (appModule != null)
                {
                    if (appModule.Forms != null)
                    {
                        ViewBag.AppId = appModule.Id.ToString();
                        ViewBag.AppName = appModule.Name;

                        ObjectId dataViewId;
                        if (ObjectId.TryParse(id, out dataViewId))
                        {
                            if (appModule.DataViews.Any(n => n.Id == dataViewId))
                            {
                                var dataView = appModule.DataViews.FirstOrDefault(n => n.Id == dataViewId);
                                response.DataView = dataView;
                                var docTypeService = new DocumentTypeServices(_appModuleRepository);
                                response.DocumentFields = await docTypeService.FindDocumentType(appId, dataView.DocumentTypeId);

                                var org = await _organisationRepository.Get(appModule.OrganisationId);

                                var dataService = new DataService(ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString, org.Id.ToString(), appModule.Name);
                                var data = await dataService.ListAll();
                                var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
                                response.Data = data.ToJson(jsonWriterSettings);

                                return View(response);
                            }
                        }

                        return View(response);
                    }
                    return View(new DataView());
                }
            }
            return View();
        }

        [Authorize]
        public async Task<ActionResult> EditTabular(string appId, string id)
        {
            if (!string.IsNullOrEmpty(appId))
            {
                var appModule = await _appModuleRepository.Get(appId);
                if (appModule != null)
                {
                    if (appModule.Forms != null)
                    {
                        ViewBag.AppId = appModule.Id.ToString();
                        ViewBag.AppName = appModule.Name;

                        ObjectId dataViewId;
                        if (ObjectId.TryParse(id, out dataViewId))
                        {
                            if (appModule.DataViews.Any(n => n.Id == dataViewId))
                            {
                                return View(appModule.DataViews.FirstOrDefault(n => n.Id == dataViewId));
                            }
                        }

                        return View(appModule.DataViews);
                    }
                    return View(new DataView());
                }
            }
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> TabularAddColumn(ReqTabularAddColumn req)
        {
            if (!string.IsNullOrEmpty(req.AppId))
            {
                var columnId = string.Empty;
                var appModule = await _appModuleRepository.Get(req.AppId);
                if (appModule != null)
                {
                    if (appModule.Forms != null)
                    {
                        ViewBag.AppId = appModule.Id.ToString();
                        ViewBag.AppName = appModule.Name;

                        ObjectId dataViewId;
                        if (ObjectId.TryParse(req.DataViewId, out dataViewId))
                        {
                            if (appModule.DataViews.Any(n => n.Id == dataViewId))
                            {
                                var dataView = appModule.DataViews.FirstOrDefault(n => n.Id == dataViewId);
                                var dataColumnId = ObjectId.GenerateNewId();
                                if (dataView.Tabular == null)
                                {
                                    dataView.Tabular = new Tabular();
                                }
                                if (dataView.Tabular.Columns == null)
                                {
                                    dataView.Tabular.Columns = new List<TabularColumns>();
                                }
                                dataView.Tabular.Columns.Add(new TabularColumns
                                {
                                    Id = dataColumnId,
                                    FieldId = ObjectId.Parse(req.FieldId),
                                    Order = req.Order
                                });

                                await _appModuleRepository.Update(req.AppId, appModule);
                                columnId = dataColumnId.ToString();
                            }
                        }                        
                    }
                    var result = new JsonGenericResult
                    {
                        IsSuccess = true,
                        Result = columnId
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

        [Authorize]
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
                            Description = req.Description,
                            DocumentTypeId = ObjectId.Parse(req.DocumentTypeId)
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