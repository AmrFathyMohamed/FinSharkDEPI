using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinShark.Models
{
    // Session 2: الـ Model (Entity) = كلاس C# بيتحوّل لجدول في الداتابيز.
    // كل property هنا = عمود في جدول Stocks.
    [Table("Stocks")]
    public class Stock
    {
        // الـ Primary Key — EF Core بيفهم إن Id هو المفتاح و Identity (auto-increment) تلقائياً
        [Key]
        public int Id { get; set; }

        // رمز السهم (AAPL مثلاً) — MaxLength بيخليه nvarchar(10) وبيسمح نعمل عليه Unique Index
        [Required]
        [MaxLength(10)]
        public string Symbol { get; set; } = string.Empty;

        // اسم الشركة
        [Required]
        [MaxLength(280)]
        public string CompanyName { get; set; } = string.Empty;

        // الفلوس دايماً decimal مش double (عشان الدقة) — و Column بيحدد نوعها في SQL بالظبط
        [Column(TypeName = "decimal(18,2)")]
        public decimal Purchase { get; set; }

        // آخر توزيع أرباح
        [Column(TypeName = "decimal(18,2)")]
        public decimal LastDiv { get; set; }

        // القطاع (Tech, Automotive...)
        [Required]
        [MaxLength(50)]
        public string Industry { get; set; } = string.Empty;

        // القيمة السوقية — long لأنها ممكن تعدّي حدود int
        public long MarketCap { get; set; }

        // Session 6: علاقة One-to-Many — السهم الواحد ليه تعليقات كتير (Navigation Property)
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
