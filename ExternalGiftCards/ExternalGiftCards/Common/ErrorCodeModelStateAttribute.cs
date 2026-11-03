using ExternalGiftCards.BusinessLayer.Common;
using ExternalGiftCards.BusinessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ExternalGiftCards.Common
{
    /// <summary>
    /// Returns model-binding errors (malformed JSON, wrong value types, empty body) as
    /// 400 { messagecode: "103", errormessage } instead of the default ProblemDetails.
    /// Runs before [ApiController]'s ModelStateInvalidFilter (Order -2000). Apply per action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ErrorCodeModelStateAttribute : Attribute, IActionFilter, IOrderedFilter
    {
        public int Order => -3000;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
            {
                return;
            }

            context.Result = new BadRequestObjectResult(new ErrorResponseDto(
                GiftCardErrorCodes.InvalidRequest,
                "Request body is missing or not valid JSON."));
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
