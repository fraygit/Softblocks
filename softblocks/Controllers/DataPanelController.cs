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
    [Authorize]
    public class DataPanelController : Controller
    {
        private IDataPanelRepository _dataPanelRepository;
        private IUserRepository _userRepository;

        public DataPanelController(IDataPanelRepository _dataPanelRepository, IUserRepository _userRepository)
        {
            this._dataPanelRepository = _dataPanelRepository;
            this._userRepository = _userRepository;
        }

        // GET: DataPanel
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> List()
        {
            var list = new List<DataPanel>();
            var user = await _userRepository.GetUser(User.Identity.Name);
            if (!string.IsNullOrEmpty(user.CurrentOrganisation))
            {
                list = await _dataPanelRepository.GetByOrg(user.CurrentOrganisation);
            }
            return View(list);
        }

        [HttpPost]
        public async Task<JsonResult> Add(DataPanel model)
        {
            try
            {
                var user = await _userRepository.GetUser(User.Identity.Name);
                if (!string.IsNullOrEmpty(user.CurrentOrganisation))
                {
                    model.OrganisationId = user.CurrentOrganisation;
                    await _dataPanelRepository.CreateSync(model);
                    var result = new JsonGenericResult
                    {
                        IsSuccess = true
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