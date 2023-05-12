using Microsoft.AspNetCore.Mvc;
using Catalog.Entities;
using Catalog.Repositories;
using Catalog.Dtos;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemController: ControllerBase
    {
        private readonly IItemRepository repository;

        public ItemController(IItemRepository repository)
        {
            this.repository = repository;
        }
         
        //GET /items 
        [HttpGet]
        public IEnumerable<ItemDto> GetItems()
        {
            var items = repository.GetItems().Select(item => item.AsDto());
            return items;
        }

        [HttpGet("{id}")]
        public ActionResult<ItemDto> GetItem(Guid id)
        {
            var item = repository.GetItem(id);

            if(item is null)
            {
                return NotFound();
            }

            return item.AsDto();
            
        }

        [HttpPost]
        public ActionResult<ItemDto> CreateItem(CreateItemDto itemDto) 
        {

            Item item = new()
            {
                Id = Guid.NewGuid(), 
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreateDate = DateTimeOffset.UtcNow
            };
                
            repository.CreateItem(item);

            return CreatedAtAction(nameof(GetItem), new { id = item.Id}, item.AsDto());
            
        }

        //UPDATE /item{id}
        [HttpPut("{id}")]

        public ActionResult<ItemDto> UpdateItem(Guid id, UpdateItemDto itemDto)
        {
            var existingItem = repository.GetItem(id);

            if (existingItem is null)
            {
                return NotFound();
            }
            // using with expression - taking a existem item and making acopy of it with the properties NAME and PRICE modified since its an immutable object
            Item updateItem = existingItem with {  
                Name = itemDto.Name,
                Price = itemDto.Price
            };

            repository.UpdateItem(updateItem);

            return updateItem.AsDto();

        }

        //DELETE /item{id}
        [HttpDelete("{id}")]

        public ActionResult DeleteItem(Guid id)
        {
            var existingItem = repository.GetItem(id);

            if (existingItem is null)
            {
                return NotFound();
            }
            repository.DeleteItem(id);
            return NoContent();
        }
    }
}