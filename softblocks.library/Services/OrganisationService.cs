using softblocks.data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.library.Services
{
    public class OrganisationService
    {
        private OrganisationRepositoty _repository;

        public OrganisationService()
        {
            _repository = new OrganisationRepositoty();
        }
    }
}
