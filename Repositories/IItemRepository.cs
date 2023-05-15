using Catalog.Entities;

namespace Catalog.Repositories
{
    public interface IItemRepository
    {

        //changing all functsion to async
        //making sure each operations when called returns a task
        //Task<""> is a way to signal that each function a now an Async method not synchronous
        //
        Task<Item> GetItemAsync(Guid id);
        Task<IEnumerable<Item>> GetItemsAsync();
        Task CreateItemAsync(Item item);
        Task UpdateItemAsync(Item item);
        Task DeleteItemAsync(Guid id);
    }
}