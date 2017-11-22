using MongoDB.Bson;
using MongoDB.Driver;
using softblocks.data.Model;
using softblocks.data.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Interface
{
    public interface IUserRepository : IEntityService<User>
    {
        Task<User> GetUser(string username);
        Task<User> GetUserByVerificationCode(string verificationCode);
        Task<List<User>> Get(ObjectId[] userId);
        Task<ReplaceOneResult> UpdateWithPassword(string id, User user);
    }
}
