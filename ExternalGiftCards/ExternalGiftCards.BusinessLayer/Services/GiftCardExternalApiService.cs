using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ExternalGiftCards.BusinessLayer.Common;
using ExternalGiftCards.BusinessLayer.Interfaces;
using ExternalGiftCards.BusinessLayer.Models.External;

namespace ExternalGiftCards.BusinessLayer.Services
{
    public class GiftCardExternalApiService : IGiftCardExternalApiService
    {
        private const int MaxLoggedBodyLength = 1000;

        private readonly HttpClient _httpClient;

        public GiftCardExternalApiService(HttpClient httpClient)
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

        public async Task<ExternalRedemptionResult> RedeemGiftCardAsync(string apiKey, string requestUri, ExternalRedemptionRequest body, CancellationToken cancellationToken = default)
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = JsonContent.Create(body)
            };
            req.Headers.Add("X-API-Key", apiKey);
            using var response = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadFromJsonAsync<ExternalRedemptionResponse>(cancellationToken);
                return new ExternalRedemptionResult { Outcome = ExternalRedemptionOutcome.Success, Response = payload };
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.Forbidden:
                    return new ExternalRedemptionResult { Outcome = ExternalRedemptionOutcome.Unauthorized };

                case HttpStatusCode.NotFound:
                    return new ExternalRedemptionResult { Outcome = ExternalRedemptionOutcome.NotFound };

                case HttpStatusCode.BadRequest:
                    var badRequestBody = await response.Content.ReadAsStringAsync(cancellationToken);
                    var error = TryParseError(badRequestBody);
                    if (error is not null)
                    {
                        return new ExternalRedemptionResult { Outcome = ExternalRedemptionOutcome.Rejected, Error = error };
                    }
                    throw new ExternalApiException(response.StatusCode, Truncate(badRequestBody));
            }

            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new ExternalApiException(response.StatusCode, Truncate(errorBody));
        }

        /// <summary>
        /// Parses { "messagecode": ..., "errormessage": ... } (case-insensitive; messagecode may be a
        /// string or a number). Returns null when the body does not match.
        /// </summary>
        private static ExternalErrorResponse? TryParseError(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                return null;
            }

            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.ValueKind != JsonValueKind.Object)
                {
                    return null;
                }

                string? code = null;
                string? message = null;
                foreach (var property in doc.RootElement.EnumerateObject())
                {
                    if (property.Name.Equals("messagecode", StringComparison.OrdinalIgnoreCase))
                    {
                        code = property.Value.ValueKind switch
                        {
                            JsonValueKind.String => property.Value.GetString(),
                            JsonValueKind.Number => property.Value.GetRawText(),
                            _ => null
                        };
                    }
                    else if (property.Name.Equals("errormessage", StringComparison.OrdinalIgnoreCase)
                             && property.Value.ValueKind == JsonValueKind.String)
                    {
                        message = property.Value.GetString();
                    }
                }

                return string.IsNullOrWhiteSpace(code)
                    ? null
                    : new ExternalErrorResponse { MessageCode = code.Trim(), ErrorMessage = message };
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private static string Truncate(string body) =>
            body.Length > MaxLoggedBodyLength ? body[..MaxLoggedBodyLength] : body;
    }
}
