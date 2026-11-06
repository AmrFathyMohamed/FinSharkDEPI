using FinShark.Models;
using Microsoft.EntityFrameworkCore;

namespace FinShark.Data
{
    // Runtime Seeder: بيشتغل وقت تشغيل التطبيق (مش مع الـ Migration زي HasData).
    // مناسب للبيانات التجريبية الكتير عشان نجرّب الـ Filtering والـ Pagination (Session 7).
    // الإعدادات من appsettings.json → SeedData. والهدف = RecordsPerTable × Multiplier.
    // Idempotent: لو العدد وصل للهدف بيتخطى، ولو أقل بيكمّل الناقص بس.
    public static class DbSeeder
    {
        private static readonly string[] Industries =
            { "Tech", "Finance", "Energy", "Health", "Retail", "Automotive", "Telecom", "Media" };

        public static async Task SeedAsync(ApplicationDBContext context, IConfiguration configuration, ILogger logger)
        {
            var section = configuration.GetSection("SeedData");
            if (!section.GetValue("Enabled", false))
                return;

            // لو فيه migrations لسه ماتنفذتش، الجداول ممكن متكونش موجودة — منعملش seed
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                logger.LogWarning("Seeding skipped: there are pending migrations. Run Update-Database first.");
                return;
            }

            var target = section.GetValue("RecordsPerTable", 100) * section.GetValue("Multiplier", 1);

            // ===== Stocks =====
            var stockCount = await context.Stocks.CountAsync();
            if (stockCount < target)
            {
                // رموز فريدة FS001, FS002... ونتخطى أي رمز موجود عشان الـ Unique Index
                var existingSymbols = await context.Stocks.Select(s => s.Symbol).ToHashSetAsync();
                var random = new Random(42);
                var newStocks = new List<Stock>();

                for (var i = 1; stockCount + newStocks.Count < target; i++)
                {
                    var symbol = $"FS{i:D3}";
                    if (existingSymbols.Contains(symbol))
                        continue;

                    newStocks.Add(new Stock
                    {
                        Symbol = symbol,
                        CompanyName = $"FinShark Company {i}",
                        Purchase = Math.Round((decimal)(random.NextDouble() * 999 + 1), 2),
                        LastDiv = Math.Round((decimal)(random.NextDouble() * 5), 2),
                        Industry = Industries[random.Next(Industries.Length)],
                        MarketCap = random.NextInt64(1_000_000, 3_000_000_000_000)
                    });
                }

                await context.Stocks.AddRangeAsync(newStocks);
                await context.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} stocks", newStocks.Count);
            }

            // ===== Comments =====
            var commentCount = await context.Comments.CountAsync();
            if (commentCount < target)
            {
                var stockIds = await context.Stocks.Select(s => s.Id).ToListAsync();
                var random = new Random(7);
                var newComments = Enumerable.Range(commentCount + 1, target - commentCount)
                    .Select(i => new Comment
                    {
                        Title = $"Comment #{i}",
                        Content = $"Sample comment number {i} about this stock.",
                        StockId = stockIds[random.Next(stockIds.Count)]
                    })
                    .ToList();

                await context.Comments.AddRangeAsync(newComments);
                await context.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} comments", newComments.Count);
            }
        }

    }
}
