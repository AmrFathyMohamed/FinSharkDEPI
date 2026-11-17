using Microsoft.Extensions.Caching.Memory;
namespace FinShark.Application.Helpers
{
    public static class CacheKeys
    {
        public static string StockById(int id) => $"stock:{id}";
    }
}
