using System.Text.Json.Serialization;

namespace GrpcService.Server.Contracts
{
    public class CategoryContract
    {
        [JsonPropertyName("categories")]
        public Category[] Categories { get; set; }
    }

    public partial class Category
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("image")]
        public string? Image { get; set; }

        [JsonPropertyName("parentCategoryName")]
        public string? ParentCategoryName { get; set; }

        [JsonPropertyName("parentCategoryId")]
        public int? ParentCategoryId { get; set; }
    }
}
