using FinShark.Application.Dtos.Stock;

namespace FinShark.Application.Validators
{
    // Validator التعديل — نفس القواعد، بنوع مختلف عشان الـ DI يسجّل IValidator<UpdateStockRequestDto>
    public class UpdateStockValidator : StockRequestValidator<UpdateStockRequestDto>
    {
    }
}
