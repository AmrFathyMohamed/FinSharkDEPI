using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinShark.Models
{
    // Identity: الـ Portfolio = الأسهم اللي الـ user حافظها عنده.
    // كل صف هنا = user واحد + سهم واحد (جدول وسيط بين AppUser و Stock)
    [Table("Portolios")]
    public class Portolio
    {
        [Key]
        public int Id { get; set; }

        // Foreign Key: صاحب الـ Portfolio (string لأن الـ Id في IdentityUser نوعه string)
        public string? AppUserId { get; set; }
        // Navigation Property: نوصل للـ user من الـ Portfolio (portfolio.AppUser.Email) من غير JOIN بإيدنا
        [ForeignKey(nameof(AppUserId))]
        public AppUser? AppUser { get; set; }

        // Foreign Key: السهم اللي في الـ Portfolio
        public int? StockId { get; set; }
        // Navigation Property: نوصل للسهم من الـ Portfolio (portfolio.Stock.Symbol) من غير JOIN بإيدنا
        [ForeignKey(nameof(StockId))]
        public Stock? Stock { get; set; }
    }
}
