using FinShark.Data;
using FinShark.Helpers;
using FinShark.Interfaces;
using FinShark.Mappers;
using FinShark.Models;
using Microsoft.EntityFrameworkCore;

namespace FinShark.Repository
{
    // Session 5: الـ Implementation — الكلاس الوحيد اللي بيكلّم الداتابيز بخصوص الأسهم.
    // الـ Controller بيطلب منه، وهو بيستخدم الـ DbContext (Separation of Concerns).
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDBContext _context;

        // الـ DbContext بييجي جاهز بالـ Dependency Injection
        public StockRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        // Session 7: Filtering + Sorting + Pagination — كله بيتنفّذ في SQL Server مش في الذاكرة
        public async Task<List<Stock>> GetAllAsync(QueryObject query)
        {
            // IQueryable: بنبني الـ query خطوة خطوة، ومش بيتنفّذ لحد ToListAsync
            // Include = Eager Loading — بنجيب التعليقات مع الأسهم في query واحدة (بيمنع مشكلة N+1)
            var stocks = _context.Stocks
                .Include(s => s.Comments)
                .AsNoTracking()   // قراءة بس، مش محتاجين Change Tracking → أسرع
                .AsQueryable();

            // ===== Filtering: كل شرط بيتضاف بس لو المستخدم بعت قيمة =====
            if (!string.IsNullOrWhiteSpace(query.Symbol))
            {
                stocks = stocks.Where(s => s.Symbol.Contains(query.Symbol));
            }

            if (!string.IsNullOrWhiteSpace(query.CompanyName))
            {
                stocks = stocks.Where(s => s.CompanyName.Contains(query.CompanyName));
            }

            if (!string.IsNullOrWhiteSpace(query.Industry))
            {
                stocks = stocks.Where(s => s.Industry == query.Industry);
            }

            // ===== Sorting: بيتحوّل لـ ORDER BY في SQL =====
            // لو SortBy مش معروف أو فاضي بنرتّب بالـ Id — لازم يكون فيه ترتيب ثابت عشان الـ Pagination تطلع صح
            stocks = query.SortBy?.Trim().ToLowerInvariant() switch
            {
                "symbol" => query.IsDescending ? stocks.OrderByDescending(s => s.Symbol) : stocks.OrderBy(s => s.Symbol),
                "companyname" => query.IsDescending ? stocks.OrderByDescending(s => s.CompanyName) : stocks.OrderBy(s => s.CompanyName),
                "purchase" => query.IsDescending ? stocks.OrderByDescending(s => s.Purchase) : stocks.OrderBy(s => s.Purchase),
                "lastdiv" => query.IsDescending ? stocks.OrderByDescending(s => s.LastDiv) : stocks.OrderBy(s => s.LastDiv),
                "marketcap" => query.IsDescending ? stocks.OrderByDescending(s => s.MarketCap) : stocks.OrderBy(s => s.MarketCap),
                _ => query.IsDescending ? stocks.OrderByDescending(s => s.Id) : stocks.OrderBy(s => s.Id)
            };

            // ===== Pagination: Skip/Take بيتحوّلوا لـ OFFSET/FETCH في SQL =====
            // صفحة 1 → Skip 0، صفحة 2 → Skip 20، ...
            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            // هنا بس الـ query بيتنفّذ فعلياً على الداتابيز
            return await stocks
                .Skip(skipNumber)
                .Take(query.PageSize)
                .ToListAsync();
        }

        // Session 6: Include عشان السهم يرجع ومعاه تعليقاته
        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await _context.Stocks
                .Include(s => s.Comments)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        // البحث بالرمز — بنوحّد شكل الرمز الأول (" aapl " → "AAPL")
        public async Task<Stock?> GetBySymbolAsync(string symbol)
        {
            var normalized = symbol.NormalizeSymbol();
            return await _context.Stocks
                .Include(s => s.Comments)
                .FirstOrDefaultAsync(s => s.Symbol == normalized);
        }

        // Add بيجهّز الإضافة، و SaveChangesAsync بينفّذ الـ INSERT — وبعدها الـ Id بيتملى من الداتابيز
        public async Task<Stock> CreateAsync(Stock stockModel)
        {
            await _context.Stocks.AddAsync(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        // Session 3: Change Tracking — بنجيب السهم، نغيّر قيمه، و EF Core بيكتشف التغيير ويعمل UPDATE لوحده
        public async Task<Stock?> UpdateAsync(int id, Stock stockModel)
        {
            var existingStock = await _context.Stocks
                .Include(s => s.Comments)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (existingStock == null)
            {
                return null;
            }

            existingStock.Symbol = stockModel.Symbol;
            existingStock.CompanyName = stockModel.CompanyName;
            existingStock.Purchase = stockModel.Purchase;
            existingStock.LastDiv = stockModel.LastDiv;
            existingStock.Industry = stockModel.Industry;
            existingStock.MarketCap = stockModel.MarketCap;

            await _context.SaveChangesAsync();

            return existingStock;
        }

        // Remove مش async (مفيش RemoveAsync) — الـ SaveChangesAsync هو اللي بيكلّم الداتابيز
        // التعليقات بتتمسح معاه بسبب الـ Cascade اللي في OnModelCreating
        public async Task<Stock?> DeleteAsync(int id)
        {
            var stockModel = await _context.Stocks.FirstOrDefaultAsync(s => s.Id == id);

            if (stockModel == null)
            {
                return null;
            }

            _context.Stocks.Remove(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        // Session 6: AnyAsync بترجّع true/false من غير ما تجيب السهم كله — أسرع
        public Task<bool> StockExists(int id)
        {
            return _context.Stocks.AnyAsync(s => s.Id == id);
        }
    }
}
