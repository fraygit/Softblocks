using softblocks.data.Interface;
using softblocks.data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.library.Services
{
    public class ModuleMenuService
    {
        private IModuleMenuRepository _moduleMenuRepository;

        public ModuleMenuService(IModuleMenuRepository _moduleMenuRepository)
        {
            this._moduleMenuRepository = _moduleMenuRepository;
        }

        public async Task<ModuleMenu> Create(ModuleMenu menu)
        {
            await _moduleMenuRepository.CreateSync(menu);
            return menu;
        }
    }
}
