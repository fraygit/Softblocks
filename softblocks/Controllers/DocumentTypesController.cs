﻿using softblocks.data.Interface;
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

        public DocumentTypesController(IDocumentTypeRepository _documentTypeRepositoy, IUserRepository _userRepository)
        {
            this._documentTypeRepositoy = _documentTypeRepositoy;
            this._userRepository = _userRepository;
        }

        // GET: DocumentTypes
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Create(string id)
        {
            var documentTypeService = new DocumentTypeService(_documentTypeRepositoy);
            var documentType = await documentTypeService.Get(id);
            return View(documentType);
        }

        [HttpPost]
        public async Task<JsonResult> Create(DocumentType model)
        {
            try
            {
                var userService = new UserService(_userRepository);
                var user = await userService.Get(User.Identity.Name);
                if (!string.IsNullOrEmpty(user.CurrentOrganisation))
                {
                    model.OrganisationId = user.CurrentOrganisation;

                    var documentTypeService = new DocumentTypeService(_documentTypeRepositoy);
                    var documentType = await documentTypeService.Create(model);

                    var result = new JsonGenericResult
                    {
                        IsSuccess = true,
                        Result = documentType.Id.ToString()
                    };
                    return Json(result);
                }
                var ErrorResult = new JsonGenericResult
                {
                    IsSuccess = false,
                    Message = "No current organisation. Please login into one."
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
    }
}