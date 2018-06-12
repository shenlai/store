using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;
using Store.Domain;

namespace SqlDataImportToMongo
{
   public   class MongoHelp
    {
       private static IMongoClient client
       {
           get
           {
               if (null == _client)
               {
                   _client = new MongoClient("mongodb://127.0.0.1:27017");
               }
               return _client;
           }
       }
        public   static IMongoDatabase database
        {
            get {
                if(null==_database)
                  _database = client.GetDatabase("Store");
                  return _database;
            }
            set {
                _database = value;
            }
        }

        public    static IMongoCollection<BsonDocument> collection
        {
           get {
               if(null==_collection)
                   _collection=database.GetCollection<BsonDocument>("Product");
               return _collection;
           }
           set {
               _collection = value;
           }
        }

        protected static IMongoClient _client;
        protected static IMongoDatabase _database;
        protected static IMongoCollection<BsonDocument> _collection;

        public async static void InsertDocuments(List<BsonDocument> docus, Type type)
        {
            try
            {
                var collection2 = database.GetCollection<BsonDocument>(type.Name);
                await collection2.InsertManyAsync(docus);
            }
            catch (Exception ex)
            {
                _client = null;
            }
           
        }

       /// <summary>
       /// 测试两种插入方式速度 
       /// </summary>
        //public async static void TestMongo()
        //{
        //    RoomInfo roomdata = new RoomInfo();
        //    List<BsonDocument> docunemts = new List<BsonDocument>();
        //    collection = database.GetCollection<BsonDocument>("HotelPersonInfo");
        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();
        //    for (int i = 1; i < 10000; i++)
        //    {
        //        var roomdatadocument = new BsonDocument(roomdata.ToBsonDocument());
        //        docunemts.Add(roomdatadocument);
        //    }
        //    //一次10000条
        //   await collection.InsertManyAsync(docunemts);
        //    sw.Stop();
        //     TimeSpan ts2 =sw.Elapsed;
        //     Console.WriteLine("total is " + ts2.TotalMilliseconds);

        //    ///一次次插 10000次
        //     Stopwatch sw2 = new Stopwatch();
        //     sw2.Start();
        //     for (int i = 1; i < 10000; i++)
        //     {
        //         var roomdatadocument = new BsonDocument(roomdata.ToBsonDocument());
        //         await collection.InsertOneAsync(roomdatadocument);
        //     }             
        //     sw2.Stop();
        //     TimeSpan ts22 = sw2.Elapsed;
        //     Console.WriteLine("total is " + ts22.TotalMilliseconds);

        // //  await collection.InsertOneAsync(roomdatadocument);
           
        //    //collection = database.GetCollection<BsonDocument>("HotelPersonInfo");
        //    // collection.InsertOneAsync(roomdatadocument);
        //}
    }
}
