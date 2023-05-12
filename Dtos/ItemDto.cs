namespace Catalog.Dtos
{
    public record ItemDto
    {
        //using init means property cannot be modified in the future
        public Guid Id { get; init; }
        public string Name { get; init; }
        public decimal Price { get; init; }
        public DateTimeOffset CreateDate { get; init; }
    }
}
 