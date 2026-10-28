using ExternalGiftCards.BusinessLayer.Common;
using Microsoft.AspNetCore.Mvc;

namespace ExternalGiftCards.Common
{
    public interface IApiResponse<T>
    {
        IActionResult ToActionResult(IServiceResponse<T> response);
        ActionResult UnauthorizedActionResult();
    }
}
