
using System.ComponentModel.DataAnnotations;

namespace FinShark.Models.Dtos
{
    public class StockRequestDto
    {
        [Required]
        [MaxLength(10,ErrorMessage = "Symbol Must Be Max 10 Chars")]
        public string Symbol { get; set; }
        [Range(1,1000000,ErrorMessage = "Purchase Must Be Between 1 to 1M")]
        public decimal Purchase { get; set; }
    }
}
