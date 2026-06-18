using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LibraryManagement.Api.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _services;
        public ValidationFilter(IServiceProvider services) => _services = services;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var arg in context.ActionArguments.Values)
            {
                if (arg is null) continue;
                var validatorType = typeof(IValidator<>).MakeGenericType(arg.GetType());
                if (_services.GetService(validatorType) is IValidator validator)
                {
                    var result = await validator.ValidateAsync(new ValidationContext<object>(arg));
                    if (!result.IsValid)
                    {
                        context.Result = new BadRequestObjectResult(new
                        {
                            errors = result.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                        });
                        return;
                    }
                }
            }
            await next();
        }
    }
}
