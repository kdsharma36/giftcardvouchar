using System.Net;
using System.Net.Http.Json;
using ExternalGiftCards.BusinessLayer.Common;
using ExternalGiftCards.BusinessLayer.Interfaces;
using ExternalGiftCards.BusinessLayer.Models.External;

namespace ExternalGiftCards.BusinessLayer.Services
{
    public class GiftCardApiService : IGiftCardApiService
    {
        private const int MaxLoggedBodyLength = 1000;

        private readonly HttpClient _httpClient;

        public GiftCardApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ExternalGiftCardResponse?> GetGiftCardAsync(string baseUrl, string id, CancellationToken cancellationToken = default)
        {
            var requestUri = BuildRequestUri(baseUrl, id);

            using var response = await _httpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                if (body.Length > MaxLoggedBodyLength)
                {
                    body = body[..MaxLoggedBodyLength];
                }
                throw new ExternalApiException(response.StatusCode, body);
            }

            return await response.Content.ReadFromJsonAsync<ExternalGiftCardResponse>(cancellationToken);
        }

        /// <summary>
        /// TODO: confirm how the provider expects the id. Currently sent as query parameter: {url}?id={id}
        /// (appended with '&amp;' if the configured URL already has a query string).
        /// </summary>
        private static Uri BuildRequestUri(string baseUrl, string id)
        {
            var separator = baseUrl.Contains('?') ? "&" : "?";
            return new Uri($"{baseUrl}{separator}id={Uri.EscapeDataString(id)}", UriKind.Absolute);
        }
    }
}
