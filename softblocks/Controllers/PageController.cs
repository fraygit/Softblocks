using MongoDB.Bson;
using softblocks.data.Interface;
using softblocks.data.Model;
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
        public async Task<ActionResult> Index(string moduleId, string pageId)
        {
            var module = await _appModuleRepository.Get(moduleId);
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
                return View(pageSelected);
            }
            return View();
        }
    }
}