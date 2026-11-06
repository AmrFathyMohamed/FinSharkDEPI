namespace FinShark.Dtos.Stock
{
    // Session 4: الـ DTO الخاص بإنشاء سهم (POST).
    // فاضي عن قصد: بيورث كل الحقول والقواعد من StockRequestDto،
    // بس وجوده كنوع منفصل بيخلّينا نسجّل Validator خاص بيه، وأي قاعدة خاصة بالإنشاء تتحط هنا.
    public class CreateStockRequestDto : StockRequestDto
    {
    }
}
