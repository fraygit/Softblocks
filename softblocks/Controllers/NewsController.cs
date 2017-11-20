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

        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize]
        public async Task<ActionResult> ViewArticle(string articleId)
        {
            ViewBag.ArticleId = articleId;
            return View();
        }

        [Authorize]
        public async Task<ActionResult> RenderContent(string articleId)
        {
            var article = await _newsRepository.Get(articleId);
            if (article != null)
            {
                var user = await _userRepository.Get(article.CreatedBy.ToString());
                if (user != null)
                {
                    ViewBag.CreatedBy = user.FirstName + " " + user.LastName;
                }
                return View(article);
            }
            return View();
        }

        [Authorize]
        public async Task<ActionResult> Edit(string articleId)
        {
            var article = await _newsRepository.Get(articleId);
            if (article != null)
            {
                return View(article);
            }
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<JsonResult> Update(ReqCreateArticle req)
        {
            var user = _userRepository.Get(User.Identity.Name);
            var article = await _newsRepository.Get(req.ArticleId);
            var oldStatus = article.Status;
            if (article != null)
            {
                article.Title = req.Title;
                article.Article = req.Article;
                article.Status = req.Status;

                if (oldStatus != "Published" && req.Status == "Published")
                {
                    article.DatePublished = DateTime.UtcNow;
                }
                await _newsRepository.Update(req.ArticleId, article);

                var result = new JsonGenericResult
                {
                    IsSuccess = true,
                    Result = req.ArticleId
                };
                return Json(result);
            }
            var resultError = new JsonGenericResult
            {
                IsSuccess = false,
                Message = "No user loggged in."
            };
            return Json(resultError);
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