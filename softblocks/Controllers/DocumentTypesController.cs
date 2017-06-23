using softblocks.data.Interface;
using softblocks.data.Model;
using softblocks.library.Services;
using softblocks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace softblocks.Controllers
{
    [Authorize]
    public class DocumentTypesController : Controller
    {
        private IDocumentTypeRepository _documentTypeRepositoy;

        public DocumentTypesController(IDocumentTypeRepository _documentTypeRepositoy)
        {
            this._documentTypeRepositoy = _documentTypeRepositoy;
        }

        // GET: DocumentTypes
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Create(DocumentType model)
        {
            try
            {
                var documentTypeService = new DocumentTypeService(_documentTypeRepositoy);
                documentTypeService.Create(model);
                var result = new JsonGenericResult
                {
                    IsSuccess = true
                };
                return Json(result);
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