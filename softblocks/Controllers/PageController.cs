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
    public class PageController : Controller
    {
        private IAppModuleRepository _appModuleRepository;
        private IUserRepository _userRepository;

        public PageController(IAppModuleRepository _appModuleRepository, IUserRepository _userRepository)
        {
            this._appModuleRepository = _appModuleRepository;
            this._userRepository = _userRepository;
        }


        // GET: Page
        [Authorize]
        public async Task<ActionResult> Index(string moduleId, string pageId, string id)
        {
            var module = await _appModuleRepository.Get(moduleId);
            var pageSelected = new AppModulePage();
            ViewBag.AppId = moduleId;

            TempData["id"] = id;

            ViewBag.Id = id;

            ObjectId pageObjectId;
            if (ObjectId.TryParse(pageId, out pageObjectId))
            {
                foreach (var page in module.Pages)
                {
                    if (page.PageId == pageObjectId)
                    {
                        pageSelected = page;
                    }
                }
                return View(pageSelected);
            }
            return View();
        }

        public async Task<JsonResult> List(string appId)
        {
            try
            {
                if (!string.IsNullOrEmpty(appId))
                {
                    var appModule = await _appModuleRepository.Get(appId);
                    if (appModule != null)
                    {
                        if (appModule.Pages == null)
                        {
                            appModule.Pages = new List<AppModulePage>();
                        }
                        var result = new JsonGenericResult
                        {
                            IsSuccess = true,
                            Result = appModule.Pages
                        };
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
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
                var ErrorResult1 = new JsonGenericResult
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
                return Json(ErrorResult1);
            }
        }
            


        [HttpPost]
        public async Task<JsonResult> AddPanel(ReqAddPanel req)
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

                        ObjectId pageId;
                        ObjectId appModuleId;

                        if (ObjectId.TryParse(req.AppModuleId, out appModuleId) && ObjectId.TryParse(req.PageId, out pageId))
                        {
                            if (appModule.Pages.Any(n => n.PageId == pageId))
                            {
                                var panelId = ObjectId.GenerateNewId();

                                var page = appModule.Pages.FirstOrDefault(n => n.PageId == pageId);
                                if (page.Panels == null)
                                {
                                    page.Panels = new List<PagePanel>();
                                }

                                var newPagePanel = new PagePanel
                                {
                                    Id = pageId,
                                    ColWidth = req.Panel.ColWidth,
                                    Order = req.Panel.Order,
                                    PanelType = req.Panel.PanelType,
                                    ForeignId = ObjectId.Parse(req.ForeignId)
                                };
                                page.Panels.Add(newPagePanel);

                                await _appModuleRepository.Update(req.AppModuleId, appModule);
                                var result = new JsonGenericResult
                                {
                                    IsSuccess = true,
                                    Result = panelId.ToString()
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

        [Authorize]
        public async Task<ActionResult> Edit(string appId, string pageId)
        {
            var module = await _appModuleRepository.Get(appId);
            var pageSelected = new AppModulePage();

            ObjectId pageObjectId;
            if (ObjectId.TryParse(pageId, out pageObjectId))
            {
                foreach (var page in module.Pages)
                {
                    if (page.PageId == pageObjectId)
                    {
                        pageSelected = page;
                    }
                }

                ViewBag.AppId = appId;
                ViewBag.AppName = module.Name;

                return View(pageSelected);
            }
            return View();
        }
    }
}