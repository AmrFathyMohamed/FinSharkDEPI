using FinShark.Application.Interfaces;
using FinShark.Domain.Entities.Models;
using FinShark.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinShark.Infrastructure.Repository
{
    // Session 6: تنفيذ عقد ICommentRepository — كل التعامل مع جدول Comments هنا
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDBContext _context;

        public CommentRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        // كل التعليقات (أو تعليقات سهم معيّن لو stockId اتبعت) — الأحدث الأول
        // AsNoTracking: قراءة بس فمش محتاجين EF يتابع التغييرات (أسرع)
        public async Task<List<Comment>> GetAllAsync(int? stockId = null)
        {
            var comments = _context.Comments
                .AsNoTracking()
                .AsQueryable();

            if (stockId.HasValue)
            {
                comments = comments.Where(c => c.StockId == stockId);
            }

            return await comments
                .OrderByDescending(c => c.CreatedOn)
                .ToListAsync();
        }

        // FindAsync بيدوّر بالـ Primary Key (وبيشوف الـ memory الأول قبل الداتابيز)
        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _context.Comments.FindAsync(id);
        }

        // بعد SaveChanges الـ Id بيتملي تلقائياً في commentModel
        public async Task<Comment> CreateAsync(Comment commentModel)
        {
            await _context.Comments.AddAsync(commentModel);
            await _context.SaveChangesAsync();
            return commentModel;
        }

        // بنعدّل Title و Content بس — الـ StockId مبيتغيرش (التعليق بيفضل على نفس السهم)
        public async Task<Comment?> UpdateAsync(int id, Comment commentModel)
        {
            var existingComment = await GetByIdAsync(id);

            if (existingComment == null)
            {
                return null;
            }

            existingComment.Title = commentModel.Title;
            existingComment.Content = commentModel.Content;

            await _context.SaveChangesAsync();

            return existingComment;
        }

        // بيرجّع null لو التعليق مش موجود → الـ Controller يرجّع 404
        public async Task<Comment?> DeleteAsync(int id)
        {
            var commentModel = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);

            if (commentModel == null)
            {
                return null;
            }

            _context.Comments.Remove(commentModel);
            await _context.SaveChangesAsync();
            return commentModel;
        }
    }
}
