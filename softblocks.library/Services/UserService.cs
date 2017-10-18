using softblocks.data.Common;
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
    public class UserService 
    {
        private IUserRepository _userRepository;

        public UserService(IUserRepository _userRepository)
        {
            this._userRepository = _userRepository;
        }

        public async Task<User> CreateUser(User user)
        {
            await _userRepository.CreateSync(user);
            return user;
        }

        public async Task<User> Get(string username)
        {
            var user = await _userRepository.GetUser(username);
            return user;
        }

        public async Task<User> ValidateUser(string username, string password)
        {
            var user = await _userRepository.GetUser(username);
            if (user != null)
            {
                if (user.Password == Crypto.HashSha256(password))
                {
                    return user;
                }
            }
            return null;
        }
    }
}
