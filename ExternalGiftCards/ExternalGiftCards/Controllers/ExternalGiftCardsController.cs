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
        private readonly IMaskingService _maskingService;

        public ExternalGiftCardsController(IGiftCardService giftCardService, IMaskingService maskingService)
        {
            _giftCardService = giftCardService;
            _maskingService = maskingService;
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
                Serilog.Log.Error(ex, "GetGiftCard failed for client {Client}, id {Id}", client, _maskingService.MaskLastFour(id));
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Redeems the specified amount from the gift card.
        /// </summary>
        /// <remarks>        /// 
        /// </remarks>
        /// <param name="client">Firma mandant.</param>
        /// <param name="request">Gift card id, amount, currency and reference number.</param>
        /// <param name="cancellationToken">Request cancellation token.</param>
        /// <response code="200">Request successful - gift card redeemed.</response>
        /// <response code="400">The request is invalid or missing mandatory fields, or rejected by the provider.</response>
        /// <response code="401">The caller is not authenticated or not authorized.</response>
        /// <response code="500">An unexpected error occurred while processing the request.</response>
        [HttpPost("giftcard/redemption")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ErrorCodeModelState]
        [ProducesResponseType(typeof(RedemptionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RedeemGiftCard([FromRoute] string client, [FromBody] RedemptionRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                IServiceResponse<RedemptionResponseDto> serviceResponse = await _giftCardService.RedeemGiftCardAsync(client, request, cancellationToken);
                return ErrorCodeResultMapper.ToActionResult(serviceResponse);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Caller disconnected; nothing useful to return.
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "RedeemGiftCard failed for client {Client}, reference {ReferenceNumber}", client, request?.ReferenceNumber);
                return ErrorCodeResultMapper.InternalServerError();
            }
        }
    }
}
