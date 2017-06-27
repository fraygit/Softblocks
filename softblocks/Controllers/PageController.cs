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
            foreach (var page in module.Pages)
            {
                if (page.PageId == pageId)
                {
                    pageSelected = page;
                }
            }
            return View(pageSelected);
        }
    }
}