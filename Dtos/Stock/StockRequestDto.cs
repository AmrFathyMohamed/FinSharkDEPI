using System.ComponentModel.DataAnnotations;

namespace FinShark.Dtos.Stock
{
    // Session 4: الحقول المشتركة بين الإنشاء والتعديل (الداخلة من الـ client).
    // abstract لأننا مش بنستخدمها مباشرة — بنستخدم CreateStockRequestDto أو UpdateStockRequestDto.
    // الـ Data Annotations هنا هي خط الدفاع الأول، و [ApiController] بيرجّع 400 تلقائياً لو اتكسرت.
    // مفيش Id هنا — الـ Id بتولّده الداتابيز.
    public abstract class StockRequestDto
    {
        [Required]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Symbol must be 1-10 characters")]
        // \s* في الأطراف: بنسمح بمسافات حوالين الرمز لأن الـ Mapper بيعمل Trim + ToUpper
        [RegularExpression(@"^\s*[A-Za-z0-9.\-]+\s*$",ErrorMessage = "Symbol can contain only letters, digits, dots, or hyphens")]
        public string Symbol { get; set; } = string.Empty;

        [Required]
        [StringLength(280, MinimumLength = 2, ErrorMessage = "Company name must be 2-280 characters")]
        public string CompanyName { get; set; } = string.Empty;

        // typeof(decimal) لأن الـ attribute مش بيقبل decimal كـ constant
        [Range(typeof(decimal), "0.01", "1000000000")]
        public decimal Purchase { get; set; }

        [Range(typeof(decimal), "0", "100")]
        public decimal LastDiv { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Industry must be 2-50 characters")]
        public string Industry { get; set; } = string.Empty;

        [Range(typeof(long), "0", "5000000000000")]
        public long MarketCap { get; set; }
    }
}
