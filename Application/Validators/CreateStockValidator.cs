using FinShark.Application.Dtos.Stock;

namespace FinShark.Application.Validators
{
    // Validator الإنشاء — بيورث كل القواعد. وجوده كنوع منفصل بيخلّي الـ DI يسجّل IValidator<CreateStockRequestDto>
    public class CreateStockValidator : StockRequestValidator<CreateStockRequestDto>
    {
    }
}
