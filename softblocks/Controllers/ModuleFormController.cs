using MongoDB.Bson;
using softblocks.data.Interface;
using softblocks.data.Model;
using softblocks.library.Services;
using softblocks.Models;
using softblocks.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace softblocks.Controllers
{
    public class ModuleFormController : Controller
    {
        private IUserRepository _userRepository;
        private IAppModuleRepository _appModuleRepository;

        public ModuleFormController(IUserRepository _userRepository, IAppModuleRepository _appModuleRepository)
        {
            this._userRepository = _userRepository;
            this._appModuleRepository = _appModuleRepository;
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

                        ObjectId moduleFormId;
                        if (ObjectId.TryParse(formId, out moduleFormId))
                        {
                            if (appModule.Forms.Any(n => n.Id == moduleFormId))
                            {
                                return View(appModule.Forms.FirstOrDefault(n => n.Id == moduleFormId));
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
                                response.DocumentFields = await docTypeService.FindDocumentType(appId, form.DocumentTypeId);
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
                                    FieldId = ObjectId.Parse(req.FieldId)
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
                            appModule.Forms.Add(new ModuleForm
                            {
                                Id = formId,
                                Name = req.Name,
                                Description = req.Description,
                                DocumentTypeId = documentTypeId
                            });
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