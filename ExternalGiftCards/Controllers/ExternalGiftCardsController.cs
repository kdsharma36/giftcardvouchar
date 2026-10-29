using Asp.Versioning;
using ExternalGiftCards.BusinessLayer.Common;
using ExternalGiftCards.BusinessLayer.Interfaces;
using ExternalGiftCards.BusinessLayer.Models;
using ExternalGiftCards.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExternalGiftCards.Controllers
{
    [Route("v{version:apiVersion}/{tenantId:tenantId}/{client}")]
    [ApiVersion("1.0")]
    [ApiController]
    [Tags("externalgiftcards")]
    public class ExternalGiftCardsController : ControllerBase
    {
        private readonly IGiftCardService _giftCardService;

        public ExternalGiftCardsController(IGiftCardService giftCardService)
        {
            _giftCardService = giftCardService;
        }

        /// <summary>
        /// Returns the external gift card detail.
        /// </summary>
        /// <remarks>
        /// The provider URL is read from p_server (key VERFIY) for the given client, then the
        /// external provider is called with the gift card id.
        /// </remarks>
        /// <param name="client">Firma mandant.</param>
        /// <param name="id">Gift card identifier.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        /// <response code="200">Gift card detail.</response>
        /// <response code="400">Missing or invalid client / id, or unknown tenant.</response>
        /// <response code="404">Provider URL not configured, or gift card not found at the provider.</response>
        /// <response code="422">Configured provider URL is invalid.</response>
        /// <response code="500">Unexpected error.</response>
        /// <response code="502">Provider error, timeout or unreachable.</response>
        [HttpGet("giftcard")]
        [ProducesResponseType(typeof(GiftCardDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<IActionResult> GetGiftCard([FromRoute] string client, [FromQuery] string id, CancellationToken cancellationToken)
        {
            try
            {
                IApiResponse<GiftCardDetailDto> apiResponse = new ApiResponse<GiftCardDetailDto>();
                IServiceResponse<GiftCardDetailDto> serviceResponse = await _giftCardService.GetGiftCardAsync(client, id, cancellationToken);
                return apiResponse.ToActionResult(serviceResponse);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Caller disconnected; nothing useful to return.
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "GetGiftCard failed for client {Client}, id {Id}", client, id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}
