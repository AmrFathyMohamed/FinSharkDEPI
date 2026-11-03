using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FinShark.Fillters
{
    public class ValidatorFilter<T> : IAsyncActionFilter where T : class
    {
        private readonly IValidator<T> _validator;

        public ValidatorFilter(IValidator<T> validator)
        {
            _validator = validator;            
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var model = context.ActionArguments.Values.OfType<T>().FirstOrDefault();
            if (model is not null) 
            {
                var result = await _validator.ValidateAsync(model,context.HttpContext.RequestAborted);
                if (!result.IsValid)
                {
                    foreach (var error in result.Errors)
                    {
                        context.ModelState.AddModelError(
                            error.PropertyName, error.ErrorMessage);
                    }
                    context.Result = new BadRequestObjectResult(
                        new ValidationProblemDetails(context.ModelState)
                        );
                    return;
                }
            }
            await next();
        }
    }
}
