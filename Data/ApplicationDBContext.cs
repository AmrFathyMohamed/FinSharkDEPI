using Microsoft.EntityFrameworkCore;
using FinShark.Models;
namespace FinShark.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options) { }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Comment> Comments { get; set; }      
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Stock>().HasData(
                            new Stock()
                            {
                                Id = 1,
                                Symbol = "ABCD",
                                CompanyName = "DEPI",
                                Purchase = 205.06m,
                                Industry = "Tech",
                                LastDiv = new DateTime(2026, 10, 10),
                                MarketCap = 250000000000,
                                IsActive = true
                            },
                            new Stock()
                            {
                                Id = 2,
                                Symbol = "EFGH",
                                CompanyName = "DECI",
                                Purchase = 505.06m,
                                Industry = "Child",
                                LastDiv = new DateTime(2025, 10, 10),
                                MarketCap = 300000000000,
                                IsActive = false
                            }
                            );

        }
    }
}
