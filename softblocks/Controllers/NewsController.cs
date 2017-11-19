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
    public class NewsController : Controller
    {
        private IUserRepository _userRepository;
        private IOrganisationRepository _organisationRepository;
        private INewsRepository _newsRepository;

        public NewsController(IUserRepository _userRepository, IOrganisationRepository _organisationRepository, INewsRepository _newsRepository)
        {
            this._newsRepository = _newsRepository;
            this._organisationRepository = _organisationRepository;
            this._userRepository = _userRepository;
        }

        // GET: News
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var user = await _userRepository.GetUser(User.Identity.Name);
            var orgId = ObjectId.Empty;
            ObjectId.TryParse(user.CurrentOrganisation, out orgId);
            var news = await _newsRepository.Get(orgId);
            return View(news);
        }

        public ActionResult Create()
        {
            return View();
        }

        public async Task<JsonResult> AddArticle(ReqCreateArticle req)
        {
            var user = await _userRepository.GetUser(User.Identity.Name);
            var orgId = ObjectId.Empty;
            ObjectId.TryParse(user.CurrentOrganisation, out orgId);

            var article = new News
            {
                Title = req.Title,
                Article = req.Article,
                Status = req.Status,
                OrganisationId = orgId,
                CreatedBy = user.Id,
                DateCreated = DateTime.UtcNow
            };
            await _newsRepository.CreateSync(article);
            var result = new JsonGenericResult
            {
                IsSuccess = true,
                Result = article.Id.ToString()
            };
            return Json(result);
        }
    }
}