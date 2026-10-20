using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinShark.Models.Dtos
{
    public class StockDto
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string CompanyName { get; set; }
        public decimal Purchase { get; set; }
        public DateTime LastDiv { get; set; }
        public string? Industry { get; set; }
        public decimal MarketCap { get; set; }
    }
}
