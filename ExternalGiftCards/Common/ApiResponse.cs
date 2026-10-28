using ExternalGiftCards.BusinessLayer.Common;
using Microsoft.AspNetCore.Mvc;

namespace ExternalGiftCards.Common
{
    public class ApiResponse<T> : IApiResponse<T>
    {
        public IActionResult ToActionResult(IServiceResponse<T> response)
        {
            switch (response.Status)
            {
                case ResponseStatus.BadRequest:
                    return new BadRequestObjectResult(response.ErrorMessage);

                case ResponseStatus.InvalidPropertyValues:
                    return new UnprocessableEntityObjectResult(response.ErrorMessage);

                case ResponseStatus.NotFound:
                    return new NotFoundObjectResult(response.ErrorMessage);

                case ResponseStatus.NoContent:
                    return new NoContentResult();

                default:
                case ResponseStatus.Ok:
                    return new OkObjectResult(response.Result);
            }
        }

        public ActionResult UnauthorizedActionResult()
        {
            return new UnauthorizedResult();
        }
    }
}
