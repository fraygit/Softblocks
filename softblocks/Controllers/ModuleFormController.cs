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