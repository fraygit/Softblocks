using MongoDB.Bson;
using MongoDB.Driver;
using softblocks.data.Common;
using softblocks.data.Interface;
using softblocks.data.Model;
using softblocks.data.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Repository
{
    public class UserRepository : EntityService<User>, IUserRepository
    {
        public async Task<User> GetUser(string username)
        {
            try
            {
                var builder = Builders<User>.Filter;
                var filter = builder.Eq("Email", username.ToLower());
                var users = await ConnectionHandler.MongoCollection.Find(filter).ToListAsync();
                if (users.Any())
                    return users.FirstOrDefault();
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<User> GetUserByVerificationCode(string verificationCode)
        {
            var builder = Builders<User>.Filter;
            var filter = builder.Eq("VerificationCode", verificationCode);
            var users = await ConnectionHandler.MongoCollection.Find(filter).ToListAsync();
            if (users.Any())
                return users.FirstOrDefault();
            return null;
        }

        public async Task<List<User>> Get(ObjectId[] userId)
        {
            var builder = Builders<User>.Filter;
            var filter = builder.In("_id", userId);
            var users = await ConnectionHandler.MongoCollection.Find(filter).ToListAsync();
            return users;
        }

        public async Task<bool> CreateSync(User user)
        {
            if (!string.IsNullOrEmpty(user.Password))
            {
                user.Password = Crypto.HashSha256(user.Password);
            }
            await ConnectionHandler.MongoCollection.InsertOneAsync(user);
            return true;
        }
    }
}
