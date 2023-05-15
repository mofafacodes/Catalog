using Catalog.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Repositories
{
    public class MongoDBItemsRepository : IItemRepository
    {
        private const string databaseName = "catalog";

        private const string collectionName = "items";

        private readonly IMongoCollection<Item> itemsCollection;

        private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter ;
        public MongoDBItemsRepository(IMongoClient mongoClient) 
        {
            //database will created on first usage if detected by the mongo drive
            // that database does not exist
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            //reference to collection
            itemsCollection = database.GetCollection<Item>(collectionName);
        }
        // using InsertOneAsync is make you're not doing blocking call that makes sure nothing happes until
        // what ever you're expecting comes back from the database
        public async Task CreateItemAsync(Item item)
        {
            await itemsCollection.InsertOneAsync(item);
        }

        public async Task DeleteItemAsync(Guid id)
        {
            var filter = filterBuilder.Eq(item => item.Id, id);
           await  itemsCollection.DeleteOneAsync(filter);
        }

        public async Task<Item> GetItemAsync(Guid id)
        {
            var filter = filterBuilder.Eq(item => item.Id, id);
            return await itemsCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            return await itemsCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task UpdateItemAsync(Item item)
        {
            var filter = filterBuilder.Eq(existingItem => existingItem.Id ,item.Id);
            await itemsCollection.ReplaceOneAsync(filter, item);
        }
    }
}
