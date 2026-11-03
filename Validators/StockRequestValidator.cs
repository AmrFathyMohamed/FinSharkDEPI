using FinShark.Dtos.Stock;
using FluentValidation;

namespace FinShark.Validators
{
    public abstract class StockRequestValidator<T> 
        : AbstractValidator<T> where T : CreateStockRequestDto
    {
        protected StockRequestValidator()
        {
            RuleFor(x => x.Symbol)
                .NotEmpty().WithMessage("Error #1")
                .MaximumLength(10).WithMessage("Error #2")
                .MinimumLength(2).WithMessage("Error #3")
                .Matches(@"^[A-Za-z0-9.\-]+$").WithMessage("Error #4");
                
        }
    }
}
