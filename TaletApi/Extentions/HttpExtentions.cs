using Azure;
using System.Text.Json;
using TaletApi.Models;
using TaletApi.RequestHelpers;

namespace TaletApi.Extentions
{
    public static class HttpExtentions
    {
        public static void AddPaginationHeader(this HttpResponse response,MetaData metaData)
        {
            var option = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData, option));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
