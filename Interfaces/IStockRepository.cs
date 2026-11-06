using FinShark.Helpers;
using FinShark.Models;

namespace FinShark.Interfaces
{
    // Session 5: الـ Interface = "عقد" بيقول إيه العمليات المتاحة على الأسهم من غير ما يقول إزاي.
    // الـ Controller بيعتمد على العقد ده مش على التنفيذ (Dependency Inversion — حرف D في SOLID).
    // علامة ? في Stock? معناها ممكن يرجّع null لو مش لاقي.
    public interface IStockRepository
    {
        // Session 7: بياخد QueryObject عشان الـ Filtering والـ Sorting والـ Pagination
        Task<List<Stock>> GetAllAsync(QueryObject query);
        Task<Stock?> GetByIdAsync(int id);
        Task<Stock?> GetBySymbolAsync(string symbol);
        Task<Stock> CreateAsync(Stock stockModel);
        Task<Stock?> UpdateAsync(int id, Stock stockModel);
        Task<Stock?> DeleteAsync(int id);
        // Session 6: نتأكد إن السهم موجود قبل ما نضيف عليه تعليق
        Task<bool> StockExists(int id);
    }
}
