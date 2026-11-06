using FinShark.Dtos.Comment;

namespace FinShark.Dtos.Stock
{
    // Session 4: شكل السهم اللي بيطلع للـ client (Response).
    // بنرجّع ده بدل الـ Entity نفسه: أمان (منسرّبش حقول داخلية) + تحكّم في الشكل + الـ API مايتكسرش لو الـ Entity اتغيّر.
    public class StockDto
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public decimal Purchase { get; set; }
        public decimal LastDiv { get; set; }
        public string Industry { get; set; } = string.Empty;
        public long MarketCap { get; set; }

        // Session 6: التعليقات كـ CommentDto (مش Comment entity) — عشان منرجعش الـ Entity
        // ومنقعش في Circular Reference (Stock → Comment → Stock → ...) وقت تحويل الـ JSON
        public List<CommentDto> Comments { get; set; } = new List<CommentDto>();
    }
}
