using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinShark.Models.Dtos
{
    public class StockDto
    {
        public int Id { get; set; }
        [MaxLength(7)]
        public string Symbol { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Purchase { get; set; }
        public DateTime LastDiv { get; set; }
        public string? Industry { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal MarketCap { get; set; }
    }
}
