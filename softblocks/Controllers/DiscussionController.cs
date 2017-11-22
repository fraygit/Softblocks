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
    public class DiscussionController : Controller
    {
        private IDiscussionRepository _discussionRepository;
        private IUserRepository _userRepository;

        public DiscussionController(IDiscussionRepository _discussionRepository, IUserRepository _userRepository)
        {
            this._discussionRepository = _discussionRepository;
            this._userRepository = _userRepository;
        }

        // GET: Discussion
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<ActionResult> ListCategory()
        {
            var user = await _userRepository.GetUser(User.Identity.Name);
            if (user != null)
            {
                var orgId = ObjectId.Empty;
                ObjectId.TryParse(user.CurrentOrganisation, out orgId);
                var categories = await _discussionRepository.GetByOrganisation(orgId);
                return View(categories);
            }
            return View();
        }

        [Authorize]
        public async Task<ActionResult> ListComments(string discussionId, string topicId)
        {
            var user = await _userRepository.GetUser(User.Identity.Name);
            if (user != null)
            {
                var orgId = ObjectId.Empty;
                ObjectId.TryParse(user.CurrentOrganisation, out orgId);

                var topicObjectId = ObjectId.Empty;
                ObjectId.TryParse(topicId, out topicObjectId);

                var discussion = await _discussionRepository.Get(discussionId);
                if (discussion != null)
                {
                    ViewBag.ParentCategoryName = discussion.Title;
                    ViewBag.DiscussionId = discussion.Id.ToString();
                    if (discussion.Topics.Any(n => n.Id == topicObjectId))
                    {
                        var topic = discussion.Topics.FirstOrDefault(n => n.Id == topicObjectId);

                        if (topic.Comments != null)
                        {
                            var response = new List<ResComment>();
                            foreach (var comment in topic.Comments.OrderByDescending(n => n.DateReplied))
                            {
                                var userComment = await _userRepository.Get(comment.UserId.ToString());
                                response.Add(new ResComment
                                {
                                    Comment = comment,
                                    Name = userComment.FirstName + " " + userComment.LastName
                                });
                            }
                            return View(response);                            
                        }
                    }
                }
            }
            return View();
        }

        [Authorize]
        public async Task<ActionResult> Topic(string discussionId, string topicId)
        {
            var user = await _userRepository.GetUser(User.Identity.Name);
            if (user != null)
            {
                var orgId = ObjectId.Empty;
                ObjectId.TryParse(user.CurrentOrganisation, out orgId);

                var topicObjectId = ObjectId.Empty;
                ObjectId.TryParse(topicId, out topicObjectId);

                var discussion = await _discussionRepository.Get(discussionId);
                if (discussion != null)
                {
                    ViewBag.ParentCategoryName = discussion.Title;
                    ViewBag.DiscussionId = discussion.Id.ToString();
                    if (discussion.Topics.Any(n => n.Id == topicObjectId))
                    {
                        var topic = discussion.Topics.FirstOrDefault(n => n.Id == topicObjectId);
                        return View(topic);
                    }
                }
            }
            return View();
        }


        [HttpPost]
        [Authorize]
        public async Task<JsonResult> Reply(ReqReply req)
        {
            var user = await _userRepository.GetUser(User.Identity.Name);
            if (user != null)
            {
                var orgId = ObjectId.Empty;
                ObjectId.TryParse(user.CurrentOrganisation, out orgId);

                var discussion = await _discussionRepository.Get(req.DiscussionId);
                if (discussion != null)
                {
                    if (discussion.Topics != null)
                    {
                        var topicId = ObjectId.Empty;
                        ObjectId.TryParse(req.TopicId, out topicId);
                        if (discussion.Topics.Any(n => n.Id == topicId))
                        {
                            var topic = discussion.Topics.FirstOrDefault(n => n.Id == topicId);
                            if (!string.IsNullOrEmpty(req.Comment))
                            {
                                if (topic.Comments == null)
                                {
                                    topic.Comments = new List<Comment>();
                                }
                                var commentId = ObjectId.GenerateNewId();
                                topic.Comments.Add(new Comment
                                {
                                    Content = req.Comment,
                                    DateReplied = DateTime.Now,
                                    Id = commentId,
                                    UserId = user.Id
                                });
                                await _discussionRepository.Update(req.DiscussionId, discussion);
                                var result = new JsonGenericResult
                                {
                                    IsSuccess = true,
                                    Result = commentId
                                };
                                return Json(result);
                            }
                        }
                    }
                }
            }
            var resultErr = new JsonGenericResult
            {
                IsSuccess = false,
                Message = "Error occured."
            };
            return Json(resultErr);
        }

        [HttpPost]
        [Authorize]
        public async Task<JsonResult> AddTopic(ReqAddCategory req)
        {
            var user = await _userRepository.GetUser(User.Identity.Name);
            if (user != null)
            {
                var orgId = ObjectId.Empty;
                ObjectId.TryParse(user.CurrentOrganisation, out orgId);

                var discussion = await _discussionRepository.Get(req.DiscussionId);
                if (discussion != null)
                {
                    if (discussion.Topics == null)
                    {
                        discussion.Topics = new List<Topic>();
                    }

                    discussion.Topics.Add(new Topic
                    {
                        Id = ObjectId.GenerateNewId(),
                        UserId = user.Id,
                        Title = req.Title,
                        Description = req.Description,
                        LastActivity = DateTime.Now                        
                    });

                    discussion.LastActivity = DateTime.Now;
                    await _discussionRepository.Update(req.DiscussionId, discussion);
                    var result = new JsonGenericResult
                    {
                        IsSuccess = true,
                        Result = req.DiscussionId
                    };
                    return Json(result);
                }

            }
            var resultErr = new JsonGenericResult
            {
                IsSuccess = false,
                Message = "Error occured."
            };
            return Json(resultErr);
        }

        [HttpPost]
        [Authorize]
        public async Task<JsonResult> AddCategory(ReqAddCategory req)
        {
            var user = await _userRepository.GetUser(User.Identity.Name);
            if (user != null)
            {
                var orgId = ObjectId.Empty;
                ObjectId.TryParse(user.CurrentOrganisation, out orgId);
                if (!string.IsNullOrEmpty(req.Title))
                {
                    var category = new Discussion
                    {
                        CreatedBy = user.Id,
                        Description = req.Description,
                        LastActivity = DateTime.UtcNow,
                        OrganisationId = orgId,
                        Title = req.Title                        
                    };
                    await _discussionRepository.CreateSync(category);
                    var result = new JsonGenericResult
                    {
                        IsSuccess = true,
                        Result = category.Id.ToString()
                    };
                    return Json(result);
                }
            }
            var resultErr = new JsonGenericResult
            {
                IsSuccess = false,
                Message = "Error occured."
            };
            return Json(resultErr);
        }
    }
}