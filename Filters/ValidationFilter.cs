using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FinShark.Filters
{
    // Session 4: Action Filter بيشغّل الـ FluentValidation قبل ما الـ action يتنفّذ.
    // بنستخدمه على الـ action كده: [TypeFilter(typeof(ValidationFilter<CreateStockRequestDto>))]
    // لو البيانات غلط → بيرجّع 400 + ValidationProblemDetails ومش بيدخل الـ action خالص.
    public class ValidationFilter<T> : IAsyncActionFilter where T : class
    {
        private readonly IValidator<T> _validator;

        // الـ Validator بييجي بالـ DI (متسجّل في Program.cs)
        public ValidationFilter(IValidator<T> validator)
        {
            _validator = validator;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // ندوّر على الـ DTO من نوع T وسط الـ parameters بتاعة الـ action
            var model = context.ActionArguments.Values.OfType<T>().FirstOrDefault();
            if (model is not null)
            {
                // RequestAborted: لو الـ client قفل الاتصال، نوقف الـ validation
                var result = await _validator.ValidateAsync(model, context.HttpContext.RequestAborted);
                if (!result.IsValid)
                {
                    foreach (var error in result.Errors)
                    {
                        context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }

                    // نقفل الطريق ونرجّع 400 بشكل قياسي فيه الأخطاء مربوطة بأسماء الحقول
                    context.Result = new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState));
                    return;
                }
            }

            // كله سليم → كمّل للـ action
            await next();
        }
    }
}
