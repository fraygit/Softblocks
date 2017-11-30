using MongoDB.Bson;
using softblocks.data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace softblocks.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IUserRepository _userRepository;
        private INewsRepository _newsRepository;
        private IOrganisationRepository _organisationRepository;

        public HomeController(IUserRepository _userRepository, INewsRepository _newsRepository, IOrganisationRepository _organisationRepository)
        {
            this._userRepository = _userRepository;
            this._newsRepository = _newsRepository;
            this._organisationRepository = _organisationRepository;
        }
        // GET: Home
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var currentUser = await _userRepository.GetUser(User.Identity.Name);
            if (!string.IsNullOrEmpty(currentUser.CurrentOrganisation))
            {
                var articles = await _newsRepository.GetStatus(ObjectId.Parse(currentUser.CurrentOrganisation), "Published");
                var organisation = await _organisationRepository.Get(currentUser.CurrentOrganisation);
                ViewBag.OrganisationName = "";
                if (organisation != null)
                {
                    ViewBag.OrganisationName = organisation.Name;
                }
                if (articles != null)
                {
                    if (articles.Any())
                    {
                        var latestArticle = articles.OrderByDescending(n => n.DatePublished).FirstOrDefault();
                        ViewBag.ArticleId = latestArticle.Id.ToString();
                    }
                }
            }
            return View(currentUser);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Login");
        }
    }
}