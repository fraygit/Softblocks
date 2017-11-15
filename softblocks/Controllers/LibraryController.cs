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
    public class LibraryController : Controller
    {
        private IFolderRepository _folderRepository;
        private IUserRepository _userRepository;

        public LibraryController(IFolderRepository _folderRepository, IUserRepository _userRepository)
        {
            this._folderRepository = _folderRepository;
            this._userRepository = _userRepository;
        }

        // GET: Library
        public async Task<ActionResult> Index(string parent)
        {
            ObjectId parentId = ObjectId.Empty;
            Folder folder;
            ViewBag.CurrentFolderName = "Root";
            if (!string.IsNullOrEmpty(parent))
            {
                ObjectId.TryParse(parent, out parentId);
                folder = await _folderRepository.Get(parentId.ToString());
                if (folder != null)
                {
                    ViewBag.CurrentFolderName = folder.Name;
                }
            }
            ViewBag.ParentId = parent;
            ViewBag.GrandParent = await GetParent(parentId);
            return View();
        }

        [Authorize]
        public async Task<ActionResult> ListFolders(string parent)
        {
            ObjectId parentId = ObjectId.Empty;
            if (!string.IsNullOrEmpty(parent))
            {
                ObjectId.TryParse(parent, out parentId);
            }
            ViewBag.ParentId = parent;
            var personalFolders = await _folderRepository.GetByParent(parentId);
            return View(personalFolders);
        }
        

        private async Task<string> GetParent(ObjectId folderId)
        {
            var folder = await _folderRepository.Get(folderId.ToString());
            if (folder != null)
            {
                return folder.Parent.ToString();
            }
            return "";
        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> AddFolder(ReqAddFolder req)
        {
            var user = await _userRepository.GetUser(User.Identity.Name);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(req.Name))
                {
                    var folder = new Folder
                    {
                        Name = req.Name,
                        Created = DateTime.UtcNow,
                        FolderType = req.FolderType,
                        CreatedBy = user.Id
                    };
                    ObjectId parentId = ObjectId.Empty;
                    if (!string.IsNullOrEmpty(req.Parent))
                    {
                        if (ObjectId.TryParse(req.Parent, out parentId))
                        {
                            folder.Parent = parentId;
                        }
                    }

                    var currentDirectory = await _folderRepository.GetByParent(parentId);
                    if (currentDirectory.Any(n => n.Name == req.Name))
                    {
                        var ErrorSamename = new JsonGenericResult
                        {
                            IsSuccess = false,
                            Message = "A folder with the same name already exists!"
                        };
                        return Json(ErrorSamename);
                    }


                    if (req.FolderType == "Personal")
                    {
                        folder.ForeignId = user.Id;
                    }
                    if (req.FolderType == "Organisation")
                    {
                        ObjectId orgId;
                        if (ObjectId.TryParse(user.CurrentOrganisation, out orgId))
                        {
                            folder.ForeignId = orgId;
                        }
                    }
                    await _folderRepository.CreateSync(folder);
                    var result = new JsonGenericResult
                    {
                        IsSuccess = true,
                        Result = folder
                    };
                    return Json(result);
                }
            }
            var ErrorResult = new JsonGenericResult
            {
                IsSuccess = false,
                Message = "No folder name specifed."
            };
            return Json(ErrorResult);
        }
    }
}