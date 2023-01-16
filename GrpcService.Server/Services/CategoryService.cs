using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcService.Server.Contracts;
using System.Text.Json;

namespace GrpcService.Server.Services
{  
    public class CategoryService : Server.CategoryService.CategoryServiceBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CategoryService> _logger;
        public CategoryService(IHttpClientFactory httpClientFactory, ILogger<CategoryService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public override async Task<CategoryResponse> GetCategories(Empty request, ServerCallContext context)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var categories = await GetCategoriesAsync(httpClient);

            var result = new CategoryResponse();

            foreach (var category in categories?.Categories)
            {
                result.Categories.Add(new Category
                {
                    Name = category.Name,
                    Id = category.Id,
                    Image = category.Image,
                    ParentCategoryId = category.ParentCategoryId,
                    ParentCategoryName = category.ParentCategoryName,
                    Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
                });
            }

            return result;
        }

        public override async Task GetCategoriesStream(Empty request, IServerStreamWriter<CategoryResponse> responseStream, ServerCallContext context)
        {
            var httpClient = _httpClientFactory.CreateClient();

            for (int i = 0; i < 30; i++)
            {
                if(context.CancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Request was cancelled");
                    break;
                }

                var categories = await GetCategoriesAsync(httpClient);
                var result = new CategoryResponse();

                foreach (var category in categories?.Categories)
                {
                    result.Categories.Add(new Category
                    {
                        Name = category.Name,
                        Id = category.Id,
                        Image = category.Image,
                        ParentCategoryId = category.ParentCategoryId,
                        ParentCategoryName = category.ParentCategoryName,
                        Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
                    });
                }
                
                await responseStream.WriteAsync(result);

                await Task.Delay(1000);
            }
        }

        private static async Task<CategoryContract> GetCategoriesAsync(HttpClient httpClient)
        {
            var responseText = await httpClient.GetStringAsync("https://localhost:7197/api/Categories");

            var categories = JsonSerializer.Deserialize<CategoryContract>(responseText);
            return categories;
        }
    }
}
