using FinShark.Models;

namespace FinShark.Interfaces
{
    // Session 6: عقد الـ Repository الخاص بالتعليقات — نفس فكرة IStockRepository من Session 5
    public interface ICommentRepository
    {
        // stockId اختياري: لو اتبعت بنرجّع تعليقات السهم ده بس
        Task<List<Comment>> GetAllAsync(int? stockId = null);
        Task<Comment?> GetByIdAsync(int id);
        Task<Comment> CreateAsync(Comment commentModel);
        Task<Comment?> UpdateAsync(int id, Comment commentModel);
        Task<Comment?> DeleteAsync(int id);
    }
}
