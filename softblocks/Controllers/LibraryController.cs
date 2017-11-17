using MongoDB.Bson;
using softblocks.data.Interface;
using softblocks.data.Model;
using softblocks.Models;
using softblocks.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace softblocks.Controllers
{
    public class LibraryController : Controller
    {
        private IFolderRepository _folderRepository;
        private ILibraryFileRepository _libraryRepository;
        private IUserRepository _userRepository;

        public LibraryController(IFolderRepository _folderRepository, IUserRepository _userRepository, ILibraryFileRepository _libraryRepository)
        {
            this._folderRepository = _folderRepository;
            this._userRepository = _userRepository;
            this._libraryRepository = _libraryRepository;
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

            var currentUser = await _userRepository.GetUser(User.Identity.Name);

            ViewBag.CurrentOrgId = currentUser.CurrentOrganisation;
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

            var currentUser = await _userRepository.GetUser(User.Identity.Name);
            var orgId = ObjectId.Empty;
            ObjectId.TryParse(currentUser.CurrentOrganisation, out orgId);

            ViewBag.ParentId = parent;
            var personalFolders = await _folderRepository.Get(parentId, currentUser.Id, orgId);
            return View(personalFolders);
        }

        [Authorize]
        public async Task<ActionResult> FilePreview(string reqFileId, int version)
        {
            var file = await _libraryRepository.Get(reqFileId);
            ViewBag.Filename = file.Filename;
            ViewBag.FileId = file.Id;
            return View(file.Versions.FirstOrDefault(n => n.Version == version));
        }

        [Authorize]
        public async Task<ActionResult> FileDetails(string reqFileId)
        {
            var file = await _libraryRepository.Get(reqFileId);
            ViewBag.UploadedByDisplayName = "";
            ViewBag.FolderId = file.FolderId;
            var currentVersion = file.Versions.OrderByDescending(n => n.Version).FirstOrDefault();
            if (currentVersion.UploadedBy != null)
            {
                var uploadedByUser = await _userRepository.Get(currentVersion.UploadedBy.ToString());
                if (uploadedByUser != null)
                {
                    ViewBag.UploadedByDisplayName = uploadedByUser.FirstName + " " + uploadedByUser.LastName;
                }
            }

            return View(file);
        }

        [Authorize]
        public async Task<ActionResult> ListFiles(string folder)
        {
            ObjectId folderId = ObjectId.Empty;
            if (!string.IsNullOrEmpty(folder))
            {
                ObjectId.TryParse(folder, out folderId);
            }

            var currentUser = await _userRepository.GetUser(User.Identity.Name);
            var orgId = ObjectId.Empty;
            ObjectId.TryParse(currentUser.CurrentOrganisation, out orgId);

            ViewBag.ParentId = folder;
            var files = await _libraryRepository.Get(folderId, currentUser.Id, orgId);
            return View(files);
        }

        [Authorize]
        public async Task<FileResult> Download(string fileId, int version, bool isEmbed)
        {
            var file = await _libraryRepository.Get(fileId);
            if (file != null)
            {
                var path = "";
                if (version == 0)
                {
                    var fileVersion = file.Versions.OrderByDescending(n => n.Version).FirstOrDefault();
                    path = fileVersion.Path;
                }
                else
                {
                    if (file.Versions.Any(n => n.Version == version))
                    {
                        var specificVersion = file.Versions.FirstOrDefault(n => n.Version == version);
                        path = specificVersion.Path;
                    }
                }

                if (!string.IsNullOrEmpty(path))
                {
                    var amazon = new AmazonService();
                    var fileByte = amazon.ReteiveFile(path);
                    if (isEmbed)
                    {
                        var cd = new System.Net.Mime.ContentDisposition
                        {
                            FileName = file.Filename,
                            Inline = true,
                        };
                        string contentType = MimeMapping.GetMimeMapping(file.Filename);
                        Response.AppendHeader("Content-Disposition", cd.ToString());
                        return File(fileByte, contentType);
                    }
                    else
                    {
                        return File(fileByte, System.Net.Mime.MediaTypeNames.Application.Octet, file.Filename);
                    }
                }
            }
            return null;
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

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> AddFile()
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;

                    var path = Server.MapPath("~/tempUpload/");
                    var reqFolderId = Request["FolderId"].ToString();
                    var reqDescription = Request["Description"].ToString();
                    var reqFolderType = Request["FileType"].ToString();

                    ObjectId folderId = ObjectId.Empty;
                    if (!string.IsNullOrEmpty(reqFolderId))
                    {
                        ObjectId.TryParse(reqFolderId, out folderId);
                    }

                    var amazon = new AmazonService();

                    for (int i = 0; i < files.Count; i++)
                    {
                        var file = files[i];
                        var tempFilePath = Path.Combine(path, file.FileName);
                        file.SaveAs(tempFilePath);
                        var s3Path = string.Format("documents/{0}", Guid.NewGuid());

                        using (var fileIO = System.IO.File.OpenRead(tempFilePath))
                        {
                            using (MemoryStream tempFileStream = new MemoryStream())
                            {
                                fileIO.CopyTo(tempFileStream);
                                amazon.S3Upload(s3Path, MimeMapping.GetMimeMapping(file.FileName), tempFileStream);
                            }
                        }
                        System.IO.File.Delete(tempFilePath);

                        var user = await _userRepository.GetUser(User.Identity.Name);
                        
                        var libraryFile = new LibraryFile();
                        var sameFilename = await _libraryRepository.Get(folderId, file.FileName);
                        if (sameFilename.Any())
                        {
                            libraryFile = sameFilename.FirstOrDefault();
                            var version = libraryFile.Versions.Count + 1;
                            libraryFile.Versions.Add(new LibraryFileVersion
                            {
                                Description = reqDescription,
                                Path = s3Path,
                                Version = version,
                                DateUploaded = DateTime.UtcNow,
                                UploadedBy = user.Id
                                
                            });
                            await _libraryRepository.Update(libraryFile.Id.ToString(), libraryFile);
                        }
                        else
                        {
                            if (user != null)
                            {
                                if (reqFolderType == "Personal")
                                {
                                    libraryFile.ForeignId = user.Id;
                                }
                                else
                                {
                                    ObjectId OrgId;
                                    if (ObjectId.TryParse(user.CurrentOrganisation, out OrgId))
                                    {
                                        libraryFile.ForeignId = OrgId;
                                    }
                                }
                            }
                            libraryFile.Filename = file.FileName;
                            libraryFile.FileType = reqFolderType;
                            libraryFile.Created = DateTime.UtcNow;
                            libraryFile.FolderId = folderId;
                            libraryFile.Versions = new List<LibraryFileVersion>();
                            libraryFile.Versions.Add(new LibraryFileVersion
                            {
                                Description = reqDescription,
                                Path = s3Path,
                                Version = 1,
                                DateUploaded = DateTime.UtcNow,
                                UploadedBy = user.Id
                            });
                            await _libraryRepository.CreateSync(libraryFile);
                        }                        
                        
                    }

                    var result = new JsonGenericResult
                    {
                        IsSuccess = true,
                        Message = ""
                    };
                    return Json(result);
                }
                catch (Exception ex)
                {
                    var ExResult = new JsonGenericResult
                    {
                        IsSuccess = false,
                        Message = ex.Message
                    };
                    return Json(ExResult);
                }
            }

            var ErrorResult = new JsonGenericResult
            {
                IsSuccess = false,
                Message = "No file specifed."
            };
            return Json(ErrorResult);
        }
    }
}