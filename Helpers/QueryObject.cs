using System.ComponentModel.DataAnnotations;

namespace FinShark.Helpers
{
    // Session 7: الـ Query Object — بيجمّع كل الـ query parameters في كلاس واحد بدل ما نكتبهم واحد واحد.
    // بيتملى تلقائياً من الـ URL بـ [FromQuery]، مثال:
    // GET /api/stock?symbol=AA&sortBy=Purchase&isDescending=true&pageNumber=1&pageSize=10
    public class QueryObject
    {
        // ===== Filtering ===== (null = مفيش فلتر)
        public string? Symbol { get; set; } = null;
        public string? CompanyName { get; set; } = null;
        public string? Industry { get; set; } = null;

        // ===== Sorting =====
        // الحقول المسموحة: Symbol, CompanyName, Purchase, LastDiv, MarketCap
        public string? SortBy { get; set; } = null;
        public bool IsDescending { get; set; } = false;

        // ===== Pagination =====
        // القيم الافتراضية: أول صفحة، 20 عنصر. والـ Range بيمنع قيم غلط (صفحة 0 أو 10000 عنصر مرة واحدة)
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 20;
    }
}
