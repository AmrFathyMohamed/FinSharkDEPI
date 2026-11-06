using FinShark.Models;
using Microsoft.EntityFrameworkCore;

namespace FinShark.Data
{
    // Session 2: الـ DbContext = البوابة اللي بنكلّم بيها الداتابيز.
    // بنورث من DbContext عشان ناخد كل قدرات EF Core (DbSet, SaveChanges, Change Tracking...)
    public class ApplicationDBContext : DbContext
    {
        // الـ options (الـ connection string ونوع الداتابيز) جاية من AddDbContext في Program.cs
        // ولازم تتبعت للـ base — من غيرها الـ DbContext مش هيعرف يتصل بأي داتابيز
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }

        // كل DbSet = جدول في الداتابيز
        public DbSet<Stock> Stocks { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;

        // OnModelCreating = المكان اللي بنظبط فيه شكل الداتابيز: العلاقات، الـ indexes، الـ constraints، والـ seed
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // ===== Stock: قيود على مستوى الداتابيز (Session 2) =====
            builder.Entity<Stock>(entity =>
            {
                // Check Constraints: حتى لو الـ API عدّى قيمة غلط، الداتابيز نفسها ترفضها
                entity.ToTable("Stocks", table =>
                {
                    table.HasCheckConstraint("CK_Stocks_Purchase", "[Purchase] > 0");
                    table.HasCheckConstraint("CK_Stocks_LastDiv", "[LastDiv] >= 0");
                    table.HasCheckConstraint("CK_Stocks_MarketCap", "[MarketCap] >= 0");
                });

                // Unique Index: مينفعش سهمين بنفس الـ Symbol
                entity.HasIndex(s => s.Symbol).IsUnique();
            });

            // ===== One-to-Many: Stock ← Comments (Session 6) =====
            // التعليق ليه سهم واحد، والسهم ليه تعليقات كتير.
            // Cascade: لما نمسح سهم، تعليقاته تتمسح معاه (بدل ما تفضل يتيمة أو الحذف يفشل)
            builder.Entity<Comment>()
                .HasOne(c => c.Stock)
                .WithMany(s => s.Comments)
                .HasForeignKey(c => c.StockId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== Seeding: بيانات مبدئية ثابتة بتتحط مع الـ Migration (Session 2) =====
            // أي تغيير هنا محتاج Migration جديد
            builder.Entity<Stock>().HasData(
                new Stock
                {
                    Id = 1, Symbol = "AAPL", CompanyName = "Apple Inc.",
                    Purchase = 150.00m, LastDiv = 0.25m,
                    Industry = "Tech", MarketCap = 2800000000000
                },
                new Stock
                {
                    Id = 2, Symbol = "TSLA", CompanyName = "Tesla Inc.",
                    Purchase = 250.00m, LastDiv = 0m,
                    Industry = "Automotive", MarketCap = 800000000000
                }
            );
        }
    }
}
