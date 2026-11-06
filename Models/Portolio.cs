using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinShark.Models
{
    [Table("Portolios")]
    public class Portolio
    {
        [Key]
        public int Id { get; set; }
        public string? AppUserId { get; set; }
        // Navigation Property: نوصل للسهم من التعليق (comment.User.Email) من غير JOIN بإيدنا
        [ForeignKey(nameof(AppUserId))]
        public AppUser? AppUser { get; set; }
        public int? StockId { get; set; }
        // Navigation Property: نوصل للسهم من التعليق (comment.Stock.Symbol) من غير JOIN بإيدنا
        [ForeignKey(nameof(StockId))]
        public Stock? Stock { get; set; }
    }
}
