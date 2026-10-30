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

        public async Task<ExternalGiftCardResponse?> GetGiftCardAsync(string apikey,string requestUri, CancellationToken cancellationToken = default)
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, requestUri);
            req.Headers.Add("X-API-Key", apikey);
            using var response = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

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
    }
}
