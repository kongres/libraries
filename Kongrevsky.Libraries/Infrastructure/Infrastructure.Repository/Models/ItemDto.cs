namespace Infrastructure.Repository.Models
{
    using Infrastructure.Repository.Attributes;

    public class ItemDto
    {
        [AccessWithoutPermission]
        public string Id { get; set; }
        [DefaultSortProperty]
        public string Name { get; set; }
    }
}