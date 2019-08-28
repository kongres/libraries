namespace Kongrevsky.Infrastructure.LogManager.Models
{
    public class CreateAdditionalInfoLogItemDto
    {
        public CreateAdditionalInfoLogItemDto(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}