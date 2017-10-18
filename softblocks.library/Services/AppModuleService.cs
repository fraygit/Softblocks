using softblocks.data.Interface;
using softblocks.data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.library.Services
{
    public class AppModuleService
    {
        private IAppModuleRepository _appModuleRepository;

        public AppModuleService(IAppModuleRepository _appModuleRepository)
        {
            this._appModuleRepository = _appModuleRepository;
        }

        public async Task<AppModule> Create(AppModule appMoodule)
        {
            await _appModuleRepository.CreateSync(appMoodule);
            return appMoodule;
        }
    }
}
