using System;
using System.Collections;
using System.Collections.Generic;
using Catalog.Entities;

namespace Catalog.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly List<Item> items = new List<Item>()
        {
            new Item { Id = Guid.NewGuid(), Name = "Potion", Price = 9, CreateDate = DateTimeOffset.UtcNow },
            new Item { Id = Guid.NewGuid(), Name = "Sword", Price = 18, CreateDate = DateTimeOffset.UtcNow },
            new Item { Id = Guid.NewGuid(), Name = "Elixir", Price = 12, CreateDate = DateTimeOffset.UtcNow }
        };

        public IEnumerable<Item> GetItems()
        {
            return items; 
        }

        public Item GetItem(Guid id)
        {
            return items.Where(item => item.Id == id).SingleOrDefault();
        }

        public void CreateItem(Item item)
        {
            items.Add(item);
        }

        public void UpdateItem(Item item)
        {
            var index = items.FindIndex(existingItem => existingItem.Id == item.Id);
            items[index] = item;
        }
        public void DeleteItem(Guid id)
        {
            var index = items.FindIndex(existingItem => existingItem.Id ==  id);
            items.RemoveAt(index);
        }
    };
};