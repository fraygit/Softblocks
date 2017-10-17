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
        public async Task<JsonResult> ListDataViews(string appId)
        {
            try
            {
                if (!string.IsNullOrEmpty(appId))
                {
                    var appModule = await _appModuleRepository.Get(appId);
                    if (appModule != null)
                    {
                        if (appModule.Forms == null)
                        {
                            appModule.DataViews = new List<DataView>();
                        }
                        var result = new JsonGenericResult
                        {
                            IsSuccess = true,
                            Result = appModule.DataViews
                        };
                        return Json(result, JsonRequestBehavior.AllowGet);
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

        [Authorize]
        public async Task<ActionResult> PreviewDetail(string appId, string id)
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

                                if (dataView.SubDocumentTypeId.HasValue)
                                {
                                    response.DocumentFields = await docTypeService.FindDocumentTypeFields(appId, dataView.SubDocumentTypeId.Value);
                                }
                                else
                                {
                                    response.DocumentFields = await docTypeService.FindDocumentTypeFields(appId, dataView.DocumentTypeId);
                                }

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

        [HttpPost]
        public async Task<JsonResult> FetchData(ReqData req)
        {
            var appModule = await _appModuleRepository.Get(req.AppId);
            var docTypeService = new DocumentTypeServices(_appModuleRepository);

            ObjectId documentTypeId;
            ObjectId.TryParse(req.DocumentTypeId, out documentTypeId);

            var documentName = await docTypeService.FindDocumentTypeName(req.AppId, documentTypeId);
            var org = await _organisationRepository.Get(appModule.OrganisationId);

            var dataService = new DataService(ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString, org.Id.ToString(), documentName);
            //var data = await dataService.Get(dataId, "");

            BsonDocument data;

            if (!string.IsNullOrEmpty(req.SubDocumentTypeId))
            {
                ObjectId subDocumentTypeId;
                ObjectId.TryParse(req.SubDocumentTypeId, out subDocumentTypeId);
                documentName = await docTypeService.FindDocumentTypeName(req.AppId, subDocumentTypeId);
                data = await dataService.Get(req.DataId, documentName);
            }
            else
            {
                data = await dataService.Get(req.DataId);
            }

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            var result = new JsonGenericResult
            {
                IsSuccess = true,
                Result = data.ToJson(jsonWriterSettings)
            };
            return Json(result);
                        
        }

        [HttpPost]
        public async Task<JsonResult> FetchListData(ReqData req)
        {
            var appModule = await _appModuleRepository.Get(req.AppId);
            var docTypeService = new DocumentTypeServices(_appModuleRepository);

            ObjectId documentTypeId;
            ObjectId.TryParse(req.DocumentTypeId, out documentTypeId);

            var documentName = await docTypeService.FindDocumentTypeName(req.AppId, documentTypeId);
            var org = await _organisationRepository.Get(appModule.OrganisationId);

            var dataService = new DataService(ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString, org.Id.ToString(), documentName);
            //var data = await dataService.Get(dataId, "");

            Object data = null;

            //if (!string.IsNullOrEmpty(req.SubDocumentTypeId))
            //{
            //    ObjectId subDocumentTypeId;
            //    ObjectId.TryParse(req.SubDocumentTypeId, out subDocumentTypeId);
            //    documentName = await docTypeService.FindDocumentTypeName(req.AppId, subDocumentTypeId);
            //    data = await dataService.Get(req.DataId, documentName);
            //}
            //else
            //{
            //    data = await dataService.ListAll();
            //}

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            if (!string.IsNullOrEmpty(req.SubDocumentTypeId))
            {
                if (req.DataId != null)
                {
                    ObjectId subDocumentTypeId;
                    ObjectId.TryParse(req.SubDocumentTypeId, out subDocumentTypeId);
                    var subDocumentName = await docTypeService.FindDocumentTypeName(req.AppId, subDocumentTypeId);

                    var parentSubDocumentName = await docTypeService.FindParentDocumentTypeName(req.AppId, documentTypeId, subDocumentName);

                    var suDocData = await dataService.ListAll(ObjectId.Parse(req.DataId), subDocumentName, parentSubDocumentName);
                    data = suDocData;
                }
            }
            else
            {
                data = await dataService.ListAll();
            }


            var result = new JsonGenericResult
            {
                IsSuccess = true,
                Result = data.ToJson(jsonWriterSettings)
            };
            return Json(result);

        }

        [Authorize]
        public async Task<ActionResult> RenderDetail(string appId, string id, string dataId)
        {
            if (!string.IsNullOrEmpty(appId))
            {
                var response = new ResRenderTabular();
                var appModule = await _appModuleRepository.Get(appId);
                if (appModule != null)
                {
                    if (appModule.DataViews != null)
                    {
                        ViewBag.AppId = appModule.Id.ToString();
                        ViewBag.AppName = appModule.Name;
                        ViewBag.DataId = dataId;
                        ViewBag.DataParentId = id;

                        ObjectId dataViewId;
                        if (ObjectId.TryParse(id, out dataViewId))
                        {
                            if (appModule.DataViews.Any(n => n.Id == dataViewId))
                            {
                                var dataView = appModule.DataViews.FirstOrDefault(n => n.Id == dataViewId);
                                response.DataView = dataView;
                                var docTypeService = new DocumentTypeServices(_appModuleRepository);

                                if (dataView.SubDocumentTypeId.HasValue)
                                {
                                    response.DocumentFields = await docTypeService.FindDocumentTypeFields(appId, dataView.SubDocumentTypeId.Value);
                                }
                                else
                                {
                                    response.DocumentFields = await docTypeService.FindDocumentTypeFields(appId, dataView.DocumentTypeId);
                                }                                

                                //var documentName = await docTypeService.FindDocumentTypeName(appId, dataView.DocumentTypeId);

                                //var org = await _organisationRepository.Get(appModule.OrganisationId);

                                //var dataService = new DataService(ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString, org.Id.ToString(), documentName);
                                ////var data = await dataService.Get(dataId, "");
                                //var data = await dataService.Get(dataId);

                                //var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
                                //response.Data = data.ToJson(jsonWriterSettings);

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
        public async Task<ActionResult> RenderDataView(string appId, string id, string isPreview, string dataId)
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
                        ViewBag.DataId = dataId;
                        ViewBag.DataParentId = id;

                        ObjectId dataViewId;
                        if (ObjectId.TryParse(id, out dataViewId))
                        {
                            if (appModule.DataViews.Any(n => n.Id == dataViewId))
                            {
                                var dataView = appModule.DataViews.FirstOrDefault(n => n.Id == dataViewId);
                                switch (dataView.DataViewType)
                                {
                                    case "Tabular":
                                        return RedirectToAction("RenderTabular", new { appId = appId, id = id, dataId = dataId });
                                        break;
                                    case "Detail":
                                        if (isPreview.ToLower() == "true")
                                        {
                                            return RedirectToAction("PreviewDetail", new { appId = appId, id = id });
                                        }
                                        else
                                        {
                                            return RedirectToAction("RenderDetail", new { appId = appId, id = id, dataId = dataId });
                                        }
                                        break;
                                }
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
                        ViewBag.DataId = dataId;

                        ObjectId dataViewId;
                        if (ObjectId.TryParse(id, out dataViewId))
                        {
                            if (appModule.DataViews.Any(n => n.Id == dataViewId))
                            {
                                var dataView = appModule.DataViews.FirstOrDefault(n => n.Id == dataViewId);
                                response.DataView = dataView;
                                var docTypeService = new DocumentTypeServices(_appModuleRepository);

                                if (dataView.SubDocumentTypeId.HasValue)
                                {
                                    response.DocumentFields = await docTypeService.FindDocumentTypeFields(appId, dataView.SubDocumentTypeId.Value);
                                    ViewBag.DocumentTypeName = await docTypeService.FindDocumentTypeName(appId, dataView.SubDocumentTypeId.Value);
                                }
                                else
                                {
                                    response.DocumentFields = await docTypeService.FindDocumentTypeFields(appId, dataView.DocumentTypeId);
                                    ViewBag.DocumentTypeName = await docTypeService.FindDocumentTypeName(appId, dataView.DocumentTypeId);
                                }

                                //var org = await _organisationRepository.Get(appModule.OrganisationId);

                                //var documentName = await docTypeService.FindDocumentTypeName(appId, dataView.DocumentTypeId);                                

                                //var dataService = new DataService(ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString, org.Id.ToString(), documentName);

                                //var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
                                //if (dataView.SubDocumentTypeId.HasValue)
                                //{
                                //    if (dataId != null)
                                //    {
                                //        var subDocumentName = await docTypeService.FindDocumentTypeName(appId, dataView.SubDocumentTypeId.Value);
                                //        var suDocData = await dataService.ListAll(ObjectId.Parse(dataId), subDocumentName);
                                //        response.Data = suDocData.ToJson(jsonWriterSettings);
                                //    }
                                //}
                                //else
                                //{
                                //    var data = await dataService.ListAll();
                                //    response.Data = data.ToJson(jsonWriterSettings);
                                //}

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

        private async Task<DataView> GetDataView(string appId, string id)
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
                                var dataView = appModule.DataViews.FirstOrDefault(n => n.Id == dataViewId);
                                var docService = new DocumentTypeServices(_appModuleRepository);
                                ViewBag.DocumentTypeName = await docService.FindDocumentTypeName(appId, dataView.DocumentTypeId);
                                ViewBag.SubDocumentTypeName = "none";
                                if (dataView.SubDocumentTypeId.HasValue)
                                {
                                    ViewBag.SubDocumentTypeName = await docService.FindDocumentTypeName(appId, dataView.SubDocumentTypeId.Value);
                                }

                                return dataView;
                            }
                        }
                    }
                }
            }
            return new DataView();
        }

        [Authorize]
        public async Task<ActionResult> EditTabular(string appId, string id)
        {
            var dataView = await GetDataView(appId, id);
            return View(dataView);
        }

        [Authorize]
        public async Task<ActionResult> EditDetail(string appId, string id)
        {
            var dataView = await GetDataView(appId, id);
            return View(dataView);
        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> DetailAddComponent(ReqDetailAddComponent req)
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
                                var componentId = ObjectId.GenerateNewId();
                                if (dataView.Detail == null)
                                {
                                    dataView.Detail = new Detail();
                                }
                                if (dataView.Detail.Components == null)
                                {
                                    dataView.Detail.Components = new List<DetailComponent>();
                                }

                                ObjectId? fieldId = null;
                                if (req.ComponentType.Contains("Data"))
                                {
                                    ObjectId fieldIdContainer;
                                    if (ObjectId.TryParse(req.FieldId, out fieldIdContainer))
                                    {
                                        fieldId = fieldIdContainer;
                                    }
                                }

                                dataView.Detail.Components.Add(new DetailComponent
                                {
                                    ColWidth = req.ColWidth,
                                    ComponentType = req.ComponentType,
                                    FieldId = fieldId,
                                    Order = req.Order,
                                    Text = req.Text,
                                    Id = componentId
                                });

                                await _appModuleRepository.Update(req.AppId, appModule);
                                columnId = componentId.ToString();
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
                                var tabularColumn = new TabularColumns
                                {
                                    Id = dataColumnId,
                                    FieldId = ObjectId.Parse(req.FieldId),
                                    Order = req.Order,
                                    IsLinkToDetailPage = req.IsLinkToDetailPage
                                };
                                if (req.IsLinkToDetailPage)
                                {
                                    tabularColumn.PageId = ObjectId.Parse(req.PageId);
                                }

                                dataView.Tabular.Columns.Add(tabularColumn);

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

                        if (!string.IsNullOrEmpty(req.SubDocumentTypeId))
                        {
                            dataView.SubDocumentTypeId = ObjectId.Parse(req.SubDocumentTypeId);
                        }

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