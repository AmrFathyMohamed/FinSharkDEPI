using FinShark.Dtos.Comment;
using FinShark.Models;

namespace FinShark.Mappers
{
    // Session 6: دوال التحويل بين Comment Entity و الـ DTOs بتاعته
    public static class CommentMappers
    {
        // Entity → DTO (للـ Response)
        public static CommentDto ToCommentDto(this Comment commentModel)
        {
            return new CommentDto
            {
                Id = commentModel.Id,
                Title = commentModel.Title,
                Content = commentModel.Content,
                CreatedOn = commentModel.CreatedOn,
                StockId = commentModel.StockId
            };
        }

        // CreateDto → Entity — الـ stockId جاي من الـ URL، وده اللي بيربط التعليق بالسهم (الـ Foreign Key)
        public static Comment ToCommentFromCreate(this CreateCommentDto commentDto, int stockId)
        {
            return new Comment
            {
                Title = commentDto.Title.Trim(),
                Content = commentDto.Content.Trim(),
                StockId = stockId
            };
        }

        // UpdateDto → Entity (Title و Content بس اللي بيتعدّلوا)
        public static Comment ToCommentFromUpdate(this UpdateCommentDto commentDto)
        {
            return new Comment
            {
                Title = commentDto.Title.Trim(),
                Content = commentDto.Content.Trim()
            };
        }
    }
}
