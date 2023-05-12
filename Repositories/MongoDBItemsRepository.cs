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
        public void CreateItem(Item item)
        {
            itemsCollection.InsertOne(item);
        }

        public void DeleteItem(Guid id)
        {
            var filter = filterBuilder.Eq(item => item.Id, id);
            itemsCollection.DeleteOne(filter);
        }

        public Item GetItem(Guid id)
        {
            var filter = filterBuilder.Eq(item => item.Id, id);
            return itemsCollection.Find(filter).SingleOrDefault();
        }

        public IEnumerable<Item> GetItems()
        {
            return itemsCollection.Find(new BsonDocument()).ToList();
        }

        public void UpdateItem(Item item)
        {
            var filter = filterBuilder.Eq(existingItem => existingItem.Id ,item.Id);
            itemsCollection.ReplaceOne(filter, item);
        }
    }
}
