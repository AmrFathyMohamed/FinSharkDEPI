using FinShark.Dtos.Stock;
using FinShark.Models;

namespace FinShark.Mappers
{
    // Session 4: دوال التحويل (Mapping) بين الـ Entity والـ DTO — مكتوبة يدوي (manual mapping).
    // كلها Extension Methods (بسبب this) فنقدر نكتب stock.ToStockDto() مباشرة.
    public static class StockMappers
    {
        // Entity → DTO (للـ Response)
        public static StockDto ToStockDto(this Stock stockModel)
        {
            return new StockDto
            {
                Id = stockModel.Id,
                Symbol = stockModel.Symbol,
                CompanyName = stockModel.CompanyName,
                Purchase = stockModel.Purchase,
                LastDiv = stockModel.LastDiv,
                Industry = stockModel.Industry,
                MarketCap = stockModel.MarketCap,
                // Session 6: التعليقات بتتحوّل لـ CommentDto (بتبقى موجودة بس لو عملنا Include)
                Comments = stockModel.Comments.Select(c => c.ToCommentDto()).ToList()
            };
        }

        // CreateDto → Entity (للـ POST)
        public static Stock ToStockFromCreateDto(this CreateStockRequestDto stockDto)
        {
            return stockDto.ToStock();
        }

        // UpdateDto → Entity (للـ PUT) — الـ Repository بياخد القيم دي ويحطها على السهم الموجود
        public static Stock ToStockFromUpdateDto(this UpdateStockRequestDto stockDto)
        {
            return stockDto.ToStock();
        }

        // توحيد شكل الـ Symbol: " aapl " تبقى "AAPL" — عشان نمنع تكرار نفس السهم بصيغ مختلفة
        public static string NormalizeSymbol(this string symbol)
        {
            return symbol.Trim().ToUpperInvariant();
        }

        // الجزء المشترك بين Create و Update
        private static Stock ToStock(this StockRequestDto stockDto)
        {
            return new Stock
            {
                Symbol = stockDto.Symbol.NormalizeSymbol(),
                CompanyName = stockDto.CompanyName.Trim(),
                Purchase = stockDto.Purchase,
                LastDiv = stockDto.LastDiv,
                Industry = stockDto.Industry.Trim(),
                MarketCap = stockDto.MarketCap
            };
        }
    }
}
