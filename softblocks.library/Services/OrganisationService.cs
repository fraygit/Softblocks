using softblocks.data.Interface;
using softblocks.data.Model;
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
        private IOrganisationRepository _organisationRepository;
        private IUserRepository _userRepository;

        public OrganisationService(IOrganisationRepository _organisationRepository, IUserRepository _userRepository)
        {
            this._organisationRepository = _organisationRepository;
            this._userRepository = _userRepository;
        }

        public async Task<Organisation> Create(Organisation organisation)
        {
            await _organisationRepository.CreateSync(organisation);
            return organisation;
        }

        public async Task<Organisation> Get(string id)
        {
            var organisation = await _organisationRepository.Get(id);
            return organisation;
        }

        public async Task<Organisation> CreateAndSetUserDefaultOrganisation(Organisation organisation, string username)
        {
            await _organisationRepository.CreateSync(organisation);

            var user = await _userRepository.GetUser(username);
            if (user != null)
            {
                user.CurrentOrganisation = organisation.Id.ToString();
            }
            await _userRepository.Update(user.Id.ToString(), user);

            return organisation;
        }

    }
}
