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
    }
}
