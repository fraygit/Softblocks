using MongoDB.Bson;
using MongoDB.Driver;
using softblocks.data.Entities.Base;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Service
{
    public class EntityService<T> : IEntityService<T> where T : IMongoEntity
    {
        protected readonly ConnectionHandler<T> ConnectionHandler;

        protected EntityService()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString; // May pass as parameter either
            ConnectionHandler = new ConnectionHandler<T>(connectionString);
        }

        public virtual void Create(T entity)
        {
            var result = ConnectionHandler.MongoCollection.InsertOneAsync(entity);
        }

        public virtual async Task<List<T>> ListAll()
        {
            return await ConnectionHandler.MongoCollection.Find(new BsonDocument()).ToListAsync();
        }

        public virtual async Task<T> Get(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
            return await ConnectionHandler.MongoCollection.Find(filter).FirstOrDefaultAsync();
        }


        public virtual async Task<ReplaceOneResult> Update(string id, T entity)
        {
            var carId = ObjectId.Parse(id);
            var builder = Builders<T>.Filter;
            var filter = builder.Eq("_id", carId);
            entity.Id = carId;
            var result = await ConnectionHandler.MongoCollection.ReplaceOneAsync(filter, entity);
            return result;
        }

        public virtual async Task<DeleteResult> Delete(string id)
        {
            var carId = ObjectId.Parse(id);
            var builder = Builders<T>.Filter;
            var filter = builder.Eq("_id", carId);
            var result = await ConnectionHandler.MongoCollection.DeleteManyAsync(filter);
            return result;
        }

        public virtual async Task<bool> CreateSync(T entity)
        {
            await ConnectionHandler.MongoCollection.InsertOneAsync(entity);
            return true;
        }

    }
}
