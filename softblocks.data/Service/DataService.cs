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

        public async Task Add(string data, string parentId, string subDocumentName)
        {
            var document = BsonSerializer.Deserialize<BsonDocument>(data);
            document.Add("_id", ObjectId.GenerateNewId());


            var parentDocument = await Get(parentId);

            if (parentDocument[subDocumentName, null] == null)
            {
                var documents = new BsonArray();
                documents.Add(document);
                parentDocument.Add(subDocumentName, documents);
            }
            else
            {
                var subDoc = parentDocument[subDocumentName].AsBsonArray;
                subDoc.Add(document);
            }

            var rootId = ObjectId.Parse(parentId);
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("_id", rootId);
            var result = await MongoCollection.ReplaceOneAsync(filter, parentDocument);
        }

        public async Task<List<BsonDocument>> ListAll()
        {
            var data = await MongoCollection.Find(new BsonDocument()).ToListAsync();
            return data;
        }

        public async Task<BsonArray> ListAll(ObjectId id, string subDocumentName)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var document = await MongoCollection.Find(filter).FirstOrDefaultAsync();

            return document[subDocumentName].AsBsonArray;
        }

        public async Task<BsonDocument> Get(string id)
        {
            //var data = await MongoCollection.Find(new BsonDocument());
            //return data;
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));
            return await MongoCollection.Find(filter).FirstOrDefaultAsync();
        }
    }
}
