using ExternalGiftCards.BusinessLayer.Common;
using ExternalGiftCards.BusinessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExternalGiftCards.Common
{
    /// <summary>
    /// Maps a service response to an action result using the { messagecode, errormessage } error contract
    /// (redemption endpoint). The GET endpoint keeps using <see cref="ApiResponse{T}"/>.
    /// </summary>
    public static class ErrorCodeResultMapper
    {
        public static IActionResult ToActionResult<T>(IServiceResponse<T> response)
        {
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return new OkObjectResult(response.Result);

                case ResponseStatus.BadRequest:
                    return new BadRequestObjectResult(new ErrorResponseDto(
                        response.ErrorCode ?? GiftCardErrorCodes.InvalidRequest,
                        string.IsNullOrWhiteSpace(response.ErrorMessage) ? GiftCardErrorCodes.InvalidRequestMessage : response.ErrorMessage));

                case ResponseStatus.Unauthorized:
                    return Unauthorized();

                default:
                    return InternalServerError();
            }
        }

        public static ObjectResult Unauthorized() =>
            new(new ErrorResponseDto(GiftCardErrorCodes.Unauthorized, GiftCardErrorCodes.UnauthorizedMessage))
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };

        public static ObjectResult InternalServerError() =>
            new(new ErrorResponseDto(GiftCardErrorCodes.UnexpectedError, GiftCardErrorCodes.UnexpectedErrorMessage))
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
    }
}
