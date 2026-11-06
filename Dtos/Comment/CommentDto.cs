namespace FinShark.Dtos.Comment
{
    // Session 6: شكل التعليق اللي بيطلع للـ client.
    // فيه StockId بس (مش الـ Stock كله) عشان نتجنّب الـ Circular Reference
    public class CommentDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public int? StockId { get; set; }
    }
}
