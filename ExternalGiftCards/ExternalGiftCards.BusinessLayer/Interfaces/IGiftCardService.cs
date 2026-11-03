using ExternalGiftCards.BusinessLayer.Common;
using ExternalGiftCards.BusinessLayer.Models;

namespace ExternalGiftCards.BusinessLayer.Interfaces
{
    public interface IGiftCardService
    {
        /// <summary>
        /// Reads the provider URL (p_server key VERFIY) for the client and returns the external gift card detail.
        /// Status: Ok, BadRequest (missing input), NotFound (URL not configured / card not found),
        /// InvalidPropertyValues (configured URL invalid), BadGateway (provider error / timeout).
        /// </summary>
        Task<IServiceResponse<GiftCardDetailDto>> GetGiftCardAsync(string client, string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads the provider redemption URL (p_server key REDEEM) and API key (PRVKEY) for the client
        /// and redeems the requested amount from the gift card.
        /// Status: Ok, BadRequest (validation / provider rejection, ErrorCode 102-105),
        /// Unauthorized (provider rejected the API key, ErrorCode 101),
        /// any other status = unexpected error (ErrorCode 100).
        /// </summary>
        Task<IServiceResponse<RedemptionResponseDto>> RedeemGiftCardAsync(string client, RedemptionRequestDto request, CancellationToken cancellationToken = default);
    }
}
