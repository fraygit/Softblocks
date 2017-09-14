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
using MongoDB.Bson;

namespace softblocks.Controllers
{
    [Authorize]
    public class DocumentTypesController : Controller
    {
        private IDocumentTypeRepository _documentTypeRepositoy;
        private IUserRepository _userRepository;
        private IAppModuleRepository _appModuleRepository;

        public DocumentTypesController(IDocumentTypeRepository _documentTypeRepositoy, IUserRepository _userRepository, IAppModuleRepository _appModuleRepository)
        {
            this._documentTypeRepositoy = _documentTypeRepositoy;
            this._userRepository = _userRepository;
            this._appModuleRepository = _appModuleRepository;
        }

        // GET: DocumentTypes
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> DropdownList()
        {
            var user = await _userRepository.GetUser(User.Identity.Name);
            if (user != null)
            {
                var documentTypes = await _documentTypeRepositoy.GetByOrg(user.CurrentOrganisation);
                if (documentTypes != null)
                {
                    return View(documentTypes);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> List(string appId)
        {
            if (!string.IsNullOrEmpty(appId))
            {
                var appModule = await _appModuleRepository.Get(appId);
                if (appModule != null)
                {
                    if (appModule.DocumentTypes != null)
                    {
                        ViewBag.AppId = appModule.Id.ToString();
                        return View(appModule.DocumentTypes);
                    }
                    return View(new List<DocumentType>());
                }
            }
            return View();
        }

        public async Task<ActionResult> Create(string id, string appId)
        {
            if (!string.IsNullOrEmpty(appId))
            {
                var appModule = await _appModuleRepository.Get(appId);
                if (appModule != null)
                {
                    if (appModule.DocumentTypes.Any(n => n.Id.ToString().ToLower() == id.ToLower()))
                    {
                        return View(appModule.DocumentTypes.FirstOrDefault(n => n.Id.ToString().ToLower() == id.ToLower()));
                    }
                }
            }
            return RedirectToAction("Index");
            
            //var documentTypeService = new DocumentTypeService(_documentTypeRepositoy);
            //var documentType = await documentTypeService.Get(id);
            //if (documentType != null)
            //{
            //    return View(documentType);
            //}
            //return RedirectToAction("Index");
        }

        public async Task<ActionResult> Fields(string id)
        {
            var documentTypeService = new DocumentTypeService(_documentTypeRepositoy);
            var documentType = await documentTypeService.Get(id);
            return View(documentType);
        }

        [HttpPost]
        public async Task<JsonResult> Create(ReqCreateDocumentType req)
        {
            try
            {
                if (!string.IsNullOrEmpty(req.AppModuleId))
                {
                    var appModule = await _appModuleRepository.Get(req.AppModuleId);
                    if (appModule != null)
                    {
                        if (appModule.DocumentTypes == null)
                        {
                            appModule.DocumentTypes = new List<DocumentType>();
                        }

                        if (appModule.DocumentTypes.Any(n => n.Name.ToLower().Trim() == req.Name.ToLower().Trim()))
                        {
                            var ErrorResult1 = new JsonGenericResult
                            {
                                IsSuccess = false,
                                Message = "A document type of the same name already exists."
                            };
                            return Json(ErrorResult1);
                        }
                        var docId = ObjectId.GenerateNewId();
                        appModule.DocumentTypes.Add(new DocumentType
                        {
                            Id = docId,
                            Name = req.Name,
                            Description = req.Description
                        });
                        await _appModuleRepository.Update(req.AppModuleId, appModule);
                        var result = new JsonGenericResult
                        {
                            IsSuccess = true,
                            Result = docId.ToString()
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

        //[HttpPost]
        //public async Task<JsonResult> Create(DocumentType model)
        //{
        //    try
        //    {
        //        var userService = new UserService(_userRepository);
        //        var user = await userService.Get(User.Identity.Name);
        //        if (!string.IsNullOrEmpty(user.CurrentOrganisation))
        //        {
        //            model.OrganisationId = user.CurrentOrganisation;

        //            var documentTypeService = new DocumentTypeService(_documentTypeRepositoy);
        //            var documentType = await documentTypeService.Create(model);

        //            var result = new JsonGenericResult
        //            {
        //                IsSuccess = true,
        //                Result = documentType.Id.ToString()
        //            };
        //            return Json(result);
        //        }
        //        var ErrorResult = new JsonGenericResult
        //        {
        //            IsSuccess = false,
        //            Message = "No current organisation. Please login into one."
        //        };
        //        return Json(ErrorResult);

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new JsonGenericResult
        //        {
        //            IsSuccess = false,
        //            Message = ex.Message
        //        });

        //    }
        //}

        [HttpPost]
        public async Task<JsonResult> AddField(ReqAddField model)
        {
            try
            {
                var documentTypeService = new DocumentTypeService(_documentTypeRepositoy);
                var field = new Field
                {
                    Name = model.Name,
                    DataType = model.DataType
                };
                var documentType = await documentTypeService.AddField(model.DocumentId, field);

                var result = new JsonGenericResult
                {
                    IsSuccess = true,
                    Result = documentType.Fields
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