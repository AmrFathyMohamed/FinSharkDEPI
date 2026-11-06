using System.ComponentModel.DataAnnotations;

namespace FinShark.Dtos.Comment
{
    // Session 6: الحقول المشتركة لإنشاء/تعديل تعليق + قواعد الـ Validation.
    // مفيش StockId هنا — السهم بييجي من الـ URL (POST /api/comment/{stockId})
    public abstract class CommentRequestDto
    {
        [Required]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Title must be 5-200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000, MinimumLength = 5, ErrorMessage = "Content must be 5-2000 characters")]
        public string Content { get; set; } = string.Empty;
    }

    // DTO الإنشاء (POST)
    public class CreateCommentDto : CommentRequestDto
    {
    }

    // DTO التعديل (PUT)
    public class UpdateCommentDto : CommentRequestDto
    {
    }
}
