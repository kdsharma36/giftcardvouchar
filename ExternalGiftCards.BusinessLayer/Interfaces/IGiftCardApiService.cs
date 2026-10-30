using ExternalGiftCards.BusinessLayer.Models.External;

namespace ExternalGiftCards.BusinessLayer.Interfaces
{
    /// <summary>
    /// Typed HttpClient wrapper for the external gift card provider.
    /// </summary>
    public interface IGiftCardApiService
    {
        /// <summary>
        /// Calls the provider's gift card detail endpoint.
        /// Returns null when the provider answers 404; throws <see cref="Common.ExternalApiException"/>
        /// for any other non-success status code.
        /// </summary>
        Task<ExternalGiftCardResponse?> GetGiftCardAsync(string apikey , string requestUri, CancellationToken cancellationToken = default);
    }
}
