using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using softblocks.data.Entities.Base;


namespace softblocks.data.Service
{
    public class ConnectionHandler<T> where T : IMongoEntity
    {
        public IMongoCollection<T> MongoCollection { get; set; }

        public ConnectionHandler(string connectionString)
        {
            try
            {
                var mongoClient = new MongoClient(connectionString);

                var defaultDb = ConfigurationManager.AppSettings["DB"];
                var db = mongoClient.GetDatabase(defaultDb);

                MongoCollection = db.GetCollection<T>(typeof(T).Name.ToLower() + "s");
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to connect to the database. Please contact your administrator.");
            }

        }
    }
}
