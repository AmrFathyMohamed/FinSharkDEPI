using FinShark.Dtos.Stock;

namespace FinShark.Validators
{
    // Validator التعديل — نفس القواعد، بنوع مختلف عشان الـ DI يسجّل IValidator<UpdateStockRequestDto>
    public class UpdateStockValidator : StockRequestValidator<UpdateStockRequestDto>
    {
    }
}
