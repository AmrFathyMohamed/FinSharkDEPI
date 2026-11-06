using FinShark.Dtos.Stock;
using FluentValidation;

namespace FinShark.Validators
{
    // Session 4 (مستوى السوق): FluentValidation — قواعد التحقق كلها في كلاس واحد ومقروءة زي الإنجليزي.
    // Generic + abstract عشان Create و Update يورثوا نفس القواعد.
    // القيد where T : StockRequestDto بيخلّي الـ compiler يعرف إن T فيه Symbol و CompanyName... إلخ.
    public abstract class StockRequestValidator<T> : AbstractValidator<T>
        where T : StockRequestDto
    {
        // protected لأن الكلاس abstract — الوارث بس اللي ينادي الـ constructor ده
        protected StockRequestValidator()
        {
            RuleFor(x => x.Symbol)
                .NotEmpty().WithMessage("Symbol is required.")
                .MaximumLength(10).WithMessage("Symbol cannot exceed 10 characters.")
                .Matches(@"^\s*[A-Za-z0-9.\-]+\s*$")
                .WithMessage("Symbol can contain only letters, digits, dots, or hyphens.");

            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Company name is required.")
                .MinimumLength(2).WithMessage("Company name must be at least 2 characters.")
                .MaximumLength(280).WithMessage("Company name cannot exceed 280 characters.")
                // Must = قاعدة مخصّصة: الاسم لازم يكون فيه حرف واحد على الأقل
                .Must(name => !string.IsNullOrWhiteSpace(name) && name.Any(char.IsLetter))
                .WithMessage("Company name must contain at least one letter.");

            RuleFor(x => x.Purchase)
                .GreaterThanOrEqualTo(0.01m).WithMessage("Purchase must be positive.")
                .LessThanOrEqualTo(1_000_000_000m);

            RuleFor(x => x.LastDiv)
                .GreaterThanOrEqualTo(0m).WithMessage("Last dividend cannot be negative.")
                .LessThanOrEqualTo(100m);

            RuleFor(x => x.Industry)
                .NotEmpty().WithMessage("Industry is required.")
                .MinimumLength(2)
                .MaximumLength(50)
                .Must(industry => !string.IsNullOrWhiteSpace(industry) && industry.Any(char.IsLetter))
                .WithMessage("Industry must contain at least one letter.");

            RuleFor(x => x.MarketCap)
                .GreaterThanOrEqualTo(0).WithMessage("Market cap cannot be negative.")
                .LessThanOrEqualTo(5_000_000_000_000);
        }
    }
}
