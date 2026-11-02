using ExternalGiftCards.BusinessLayer.Models.External;

namespace ExternalGiftCards.BusinessLayer.Interfaces
{
    /// <summary>
    /// Typed HttpClient wrapper for the external gift card provider.
    /// </summary>
    public interface IGiftCardExternalApiService
    {
        /// <summary>
        /// Calls the provider's gift card detail endpoint.
        /// Returns null when the provider answers 404; throws <see cref="Common.ExternalApiException"/>
        /// for any other non-success status code.
        /// </summary>
        Task<ExternalGiftCardResponse?> GetGiftCardAsync(string apikey , string requestUri, CancellationToken cancellationToken = default);

        /// <summary>
        /// POSTs a redemption to the provider. Returns Success / Rejected (400 with messagecode body) /
        /// Unauthorized (401, 403) / NotFound (404); throws <see cref="Common.ExternalApiException"/>
        /// for any other non-success status code. Never retried (POST is not idempotent).
        /// </summary>
        Task<ExternalRedemptionResult> RedeemGiftCardAsync(string apiKey, string requestUri, ExternalRedemptionRequest body, CancellationToken cancellationToken = default);
    }
}
