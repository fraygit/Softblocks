using MongoDB.Bson;
using softblocks.data.Interface;
using softblocks.data.Model;
using softblocks.data.Service;
using softblocks.library.Services;
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
    public class ModuleFormController : Controller
    {
        private IUserRepository _userRepository;
        private IAppModuleRepository _appModuleRepository;
        private IOrganisationRepository _organisationRepository;

        public ModuleFormController(IUserRepository _userRepository, IAppModuleRepository _appModuleRepository, IOrganisationRepository _organisationRepository)
        {
            this._userRepository = _userRepository;
            this._appModuleRepository = _appModuleRepository;
            this._organisationRepository = _organisationRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> EditForm(string appId, string formId)
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


                        ObjectId moduleFormId;
                        if (ObjectId.TryParse(formId, out moduleFormId))
                        {
                            if (appModule.Forms.Any(n => n.Id == moduleFormId))
                            {
                                var form = appModule.Forms.FirstOrDefault(n => n.Id == moduleFormId);
                                var docSrvc = new DocumentTypeServices(_appModuleRepository);
                                var documentName = await docSrvc.FindDocumentTypeName(appId, form.DocumentTypeId);
                                ViewBag.SubDocumentName = "None";
                                if (form.SubDocumentTypeId != null)
                                {
                                    var sudDocName = await docSrvc.FindDocumentTypeName(appId, form.SubDocumentTypeId.Value);
                                    ViewBag.SubDocumentName = sudDocName;
                                }
                                ViewBag.DocumentName = documentName;
                                return View(form);
                            }
                        }

                        return View(appModule.Forms);
                    }
                    return View(new ModuleForm());
                }
            }
            return View();
        }

        public async Task<ActionResult> RenderForm(string appId, string reqFormId)
        {
            if (!string.IsNullOrEmpty(appId))
            {
                var appModule = await _appModuleRepository.Get(appId);
                if (appModule != null)
                {
                    if (appModule.Forms != null)
                    {
                        ViewBag.AppId = appModule.Id.ToString();

                        ObjectId formId;

                        if (ObjectId.TryParse(reqFormId, out formId))
                        {
                            if (appModule.Forms.Any(n => n.Id == formId))
                            {
                                var form = appModule.Forms.FirstOrDefault(n => n.Id == formId);
                                var response = new ResRenderForm();
                                response.Form = form;

                                var docTypeService = new DocumentTypeServices(_appModuleRepository);
                                if (form.SubDocumentTypeId != null && form.SubDocumentTypeId != ObjectId.Empty)
                                {
                                    response.DocumentFields = await docTypeService.FindDocumentTypeFields(appId, form.SubDocumentTypeId.Value);
                                }
                                else
                                {
                                    response.DocumentFields = await docTypeService.FindDocumentTypeFields(appId, form.DocumentTypeId);
                                }
                                return View(response);
                            }
                        }
                        
                    }
                    return View(new ResRenderForm());
                }
            }
            return View(new List<ModuleForm>());
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
                        return View(appModule.Forms);
                    }
                    return View(new List<ModuleForm>());
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Insert(ReqAddData req)
        {
            try
            {
                if (!string.IsNullOrEmpty(req.appId))
                {
                    var appModule = await _appModuleRepository.Get(req.appId);
                    if (appModule != null)
                    {
                        if (appModule.Forms == null)
                        {
                            appModule.Forms = new List<ModuleForm>();
                        }

                        ObjectId formId;
                        if (ObjectId.TryParse(req.foreignId, out formId))
                        {
                            var form =  appModule.Forms.FirstOrDefault(n => n.Id == formId);
                            var result = new JsonGenericResult
                            {
                                IsSuccess = true,
                                Result = form
                            };

                            var org = await _organisationRepository.Get(appModule.OrganisationId);

                            var docTypeService = new DocumentTypeServices(_appModuleRepository);
                            var documentName = await docTypeService.FindDocumentTypeName(req.appId, form.DocumentTypeId);

                            var parentDocumentName = documentName == req.ParentDocumentName ? "" : req.ParentDocumentName;


                            var dataService = new DataService(ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString, org.Id.ToString(), documentName);
                            var serializer = new JavaScriptSerializer();

                            if (form.SubDocumentTypeId != null && form.SubDocumentTypeId != ObjectId.Empty)
                            {
                                var subDocumentName = await docTypeService.FindDocumentTypeName(req.appId, form.SubDocumentTypeId.Value);
                                await dataService.Add(req.data, req.RootDataId, subDocumentName, parentDocumentName);
                            }
                            else
                            {
                                dataService.Add(req.data);
                            }

                            return Json(result);
                        }
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


        public async Task<JsonResult> ListForms(string appId)
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
                            appModule.Forms = new List<ModuleForm>();
                        }
                        var result = new JsonGenericResult
                        {
                            IsSuccess = true,
                            Result = appModule.Forms
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

        [HttpPost]
        public async Task<JsonResult> AddFormField(ReqAddFormField req)
        {
            try
            {
                if (!string.IsNullOrEmpty(req.AppModuleId))
                {
                    var appModule = await _appModuleRepository.Get(req.AppModuleId);
                    if (appModule != null)
                    {
                        if (appModule.Forms == null)
                        {
                            appModule.Forms = new List<ModuleForm>();
                        }

                        ObjectId formId;
                        ObjectId appModuleId;

                        if (ObjectId.TryParse(req.AppModuleId, out appModuleId) && ObjectId.TryParse(req.FormId, out formId))
                        {
                            if (appModule.Forms.Any(n => n.Id == formId))
                            {
                                var formFieldId = ObjectId.GenerateNewId();

                                var form = appModule.Forms.FirstOrDefault(n => n.Id == formId);
                                if (form.Fields == null)
                                {
                                    form.Fields = new List<FormField>();
                                }

                                var newFormField = new FormField
                                {
                                    Id = formFieldId,
                                    FieldId = ObjectId.Parse(req.FieldId),
                                    Order = req.Order,
                                    ColWidth = req.ColWidth
                                };
                                form.Fields.Add(newFormField);

                                await _appModuleRepository.Update(req.AppModuleId, appModule);
                                var result = new JsonGenericResult
                                {
                                    IsSuccess = true,
                                    Result = formFieldId.ToString()
                                };
                                return Json(result);
                            }
                        }
                        var ErrorResult2 = new JsonGenericResult
                        {
                            IsSuccess = false,
                            Message = "Invalid app module id or document type id."
                        };
                        return Json(ErrorResult2);
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

        [HttpPost]
        public async Task<JsonResult> Create(ReqCreateForm req)
        {
            try
            {
                if (!string.IsNullOrEmpty(req.AppModuleId))
                {
                    var appModule = await _appModuleRepository.Get(req.AppModuleId);
                    if (appModule != null)
                    {
                        if (appModule.Forms == null)
                        {
                            appModule.Forms = new List<ModuleForm>();
                        }

                        ObjectId documentTypeId;
                        ObjectId appModuleId;

                        if (ObjectId.TryParse(req.AppModuleId, out appModuleId) && ObjectId.TryParse(req.DocumentTypeId, out documentTypeId))
                        {

                            if (appModule.Forms.Any(n => n.Name.ToLower().Trim() == req.Name.ToLower().Trim()))
                            {
                                var ErrorResult1 = new JsonGenericResult
                                {
                                    IsSuccess = false,
                                    Message = "A Form of the same name already exists."
                                };
                                return Json(ErrorResult1);
                            }
                            var formId = ObjectId.GenerateNewId();
                            var newForm = new ModuleForm
                            {
                                Id = formId,
                                Name = req.Name,
                                Description = req.Description,
                                DocumentTypeId = documentTypeId,
                                SubDocumentTypeId = null
                            };

                            if (!string.IsNullOrEmpty(req.SubDocumentTypeId))
                            {
                                ObjectId subDocumentTypeId;
                                if (ObjectId.TryParse(req.SubDocumentTypeId, out subDocumentTypeId))
                                {
                                    newForm.SubDocumentTypeId = subDocumentTypeId;
                                }
                            }

                            appModule.Forms.Add(newForm);
                            await _appModuleRepository.Update(req.AppModuleId, appModule);
                            var result = new JsonGenericResult
                            {
                                IsSuccess = true,
                                Result = formId.ToString()
                            };
                            return Json(result);
                        }
                        var ErrorResult2 = new JsonGenericResult
                        {
                            IsSuccess = false,
                            Message = "Invalid app module id or document type id."
                        };
                        return Json(ErrorResult2);
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