using MongoDB.Driver;

namespace backend.Tests.Shared
{
    public static class TestMongo
    {
        private static readonly IMongoClient _client =
            new MongoClient("mongodb://localhost:27017");

        public static IMongoClient Client => _client;

        public static IMongoDatabase GetDatabase(string name = "WarehouseTestDb")
        {
            return _client.GetDatabase(name);
        }
    }
}