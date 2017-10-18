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

        //private void UpdateSubDocument(BsonDocument parentDocument, string parentDocumentName, BsonDocument document, string parentId)
        //{
        //    foreach (BsonElement field in parentDocument)
        //    {
        //        if (field.Name == parentDocumentName)
        //        {
        //            var parentSubDocuments = field.Value as BsonArray;
        //            foreach (BsonDocument parentSubDocument in parentSubDocuments)
        //            {
        //                var a = parentSubDocument["_id"].ToString();
        //                if (parentSubDocument["_id"].ToString() == parentId)
        //                {
        //                    if (parentSubDocument[subDocumentName, null] == null)
        //                    {
        //                        var newSubDocument = new BsonArray();
        //                        newSubDocument.Add(document);
        //                        parentSubDocument.Add(subDocumentName, document);
        //                    }
        //                    else
        //                    {
        //                        var subDoc2 = parentSubDocument[subDocumentName].AsBsonArray;
        //                        subDoc2.Add(document);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        public async Task Add(string data, string parentId, string subDocumentName, string parentDocumentName)
        {
            var document = BsonSerializer.Deserialize<BsonDocument>(data);
            document.Add("_id", ObjectId.GenerateNewId());


            //var parentDocument = await Get(parentId);
            var parentDocument = string.IsNullOrEmpty(parentDocumentName) ? await Get(parentId) : await Get(parentId, parentDocumentName);

            if (string.IsNullOrEmpty(parentDocumentName))
            {
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
            }
            else
            {
                foreach (BsonElement field in parentDocument)
                {
                    if (field.Name == parentDocumentName)
                    {
                        var parentSubDocuments = field.Value as BsonArray;
                        foreach (BsonDocument parentSubDocument in parentSubDocuments)
                        {
                            var a = parentSubDocument["_id"].ToString();
                            if (parentSubDocument["_id"].ToString() == parentId)
                            {
                                if (parentSubDocument[subDocumentName, null] == null)
                                {
                                    var newSubDocument = new BsonArray();
                                    newSubDocument.Add(document);
                                    parentSubDocument.Add(subDocumentName, newSubDocument);
                                }
                                else
                                {
                                    var subDoc2 = parentSubDocument[subDocumentName].AsBsonArray;
                                    subDoc2.Add(document);
                                }
                            }
                        }
                    }
                    else
                    {
                        var type = field.GetType().ToString();
                    }
                }

            }

            //var rootId = ObjectId.Parse(parentId);
            var rootId = parentDocument["_id"].AsObjectId;
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("_id", rootId);
            var result = await MongoCollection.ReplaceOneAsync(filter, parentDocument);
        }

        public async Task<List<BsonDocument>> ListAll()
        {
            var data = await MongoCollection.Find(new BsonDocument()).ToListAsync();
            return data;
        }

        public async Task<BsonArray> ListAll(ObjectId id, string subDocumentName, string parentDocumentName)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var document = await MongoCollection.Find(filter).FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(parentDocumentName))
            {
                var filter2 = Builders<BsonDocument>.Filter.Eq(parentDocumentName + "._id", id);
                var parentDocument = await MongoCollection.Find(filter2).FirstOrDefaultAsync();
                var nested = GetSubDocument(parentDocument, id, subDocumentName);
                if (nested != null)
                {
                    return nested;
                }
            }


            return document[subDocumentName].AsBsonArray;


            //var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            //var document = await MongoCollection.Find(filter).FirstOrDefaultAsync();
            //return GetSubDocument(document, id, subDocumentName);
        }

        private BsonArray GetSubDocument(BsonDocument document, ObjectId parentId, string subDocumentName)
        {
            foreach (BsonElement element in document)
            {
                if (element.Name == "_id")
                {
                    if (element.Value.ToString() == parentId.ToString())
                    {
                        return document[subDocumentName].AsBsonArray;
                    }
                }
                else
                {
                    if (element.Value.IsBsonArray)
                    {
                        foreach (BsonDocument subElem in element.Value.AsBsonArray)
                        {
                            var nestedSubDocument = GetSubDocument(subElem, parentId, subDocumentName);
                            if (nestedSubDocument != null)
                            {
                                return nestedSubDocument;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public async Task<BsonDocument> Get(string id)
        {
            //var data = await MongoCollection.Find(new BsonDocument());
            //return data;
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));
            return await MongoCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<BsonDocument> Get(string id, string subDocumentName)
        {
            var filter = Builders<BsonDocument>.Filter.Eq(subDocumentName + "._id", ObjectId.Parse(id));
            var document = await MongoCollection.Find(filter).FirstOrDefaultAsync();
            return document;
        }
    }
}
