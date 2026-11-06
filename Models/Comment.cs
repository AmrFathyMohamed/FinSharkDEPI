using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinShark.Models
{
    // Session 6: موديل التعليق — الطرف "المتعدّد" في علاقة One-to-Many مع Stock
    [Table("Comments")]
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        // وقت إنشاء التعليق — بنستخدم UTC عشان الوقت يبقى موحّد مهما كان مكان السيرفر
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        // ===== العلاقة مع Stock (One-to-Many) =====
        // Foreign Key: بيقول التعليق ده على أنهي سهم
        public int? StockId { get; set; }
        // Navigation Property: نوصل للسهم من التعليق (comment.Stock.Symbol) من غير JOIN بإيدنا
        public Stock? Stock { get; set; }
    }
}
