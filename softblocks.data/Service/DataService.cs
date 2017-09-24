using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.data.Service
{
    public class DataService
    {
        public IMongoCollection<BsonDocument> MongoCollection { get; set; }

        public DataService(string connectionString, string database, string collectionName)
        {
            try
            {
                var mongoClient = new MongoClient(connectionString);

                var defaultDb = database;
                var db = mongoClient.GetDatabase(defaultDb);

                MongoCollection = db.GetCollection<BsonDocument>(collectionName);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to connect to the database. Please contact your administrator.");
            }
        }

        public void Add(string data)
        {
            var document = BsonSerializer.Deserialize<BsonDocument>(data);
            MongoCollection.InsertOneAsync(document);
        }

        public async Task<List<BsonDocument>> ListAll()
        {
            var data = await MongoCollection.Find(new BsonDocument()).ToListAsync();
            return data;
        }
    }
}
