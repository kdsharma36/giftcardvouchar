using System.Text.Json;
using ExternalGiftCards.BusinessLayer.Common;
using ExternalGiftCards.BusinessLayer.Interfaces;
using ExternalGiftCards.BusinessLayer.MappingProfile;
using ExternalGiftCards.BusinessLayer.Models;
using Microsoft.Extensions.Logging;

namespace ExternalGiftCards.BusinessLayer.Services
{
    public class GiftCardService : IGiftCardService
    {
        private const string ProviderErrorMessage = "The external gift card provider could not be reached or returned an error.";

        private readonly IPServerService _pServerService;
        private readonly IGiftCardApiService _giftCardApiService;
        private readonly ILogger<GiftCardService> _logger;

        public GiftCardService(IPServerService pServerService, IGiftCardApiService giftCardApiService, ILogger<GiftCardService> logger)
        {
            _pServerService = pServerService;
            _giftCardApiService = giftCardApiService;
            _logger = logger;
        }

        public async Task<IServiceResponse<GiftCardDetailDto>> GetGiftCardAsync(string client, string id, CancellationToken cancellationToken = default)
        {
            var response = new ServiceResponse<GiftCardDetailDto>();

            // 1. Validate input
            if (string.IsNullOrWhiteSpace(client) || string.IsNullOrWhiteSpace(id))
            {
                response.Status = ResponseStatus.BadRequest;
                response.ErrorMessage = "'client' and 'id' are required.";
                return response;
            }

            client = client.Trim();
            id = id.Trim();

            // 2. Read the provider URL from p_server (key VERFIY)
            var pServer = await _pServerService.GetPServerAsync(
                client, GiftCardProviderConstants.PServerMc, GiftCardProviderConstants.VerifyUrlKey, cancellationToken);

            if (pServer.Status != ResponseStatus.Ok || string.IsNullOrWhiteSpace(pServer.Result?.Value))
            {
                _logger.LogWarning("Gift card provider URL (key {Key}) not configured for client {Client}, mc {Mc}",
                    GiftCardProviderConstants.VerifyUrlKey, client, GiftCardProviderConstants.PServerMc);
                response.Status = ResponseStatus.NotFound;
                response.ErrorMessage = $"Gift card provider URL ('{GiftCardProviderConstants.VerifyUrlKey}') is not configured for client '{client}'.";
                return response;
            }

            var url = pServer.Result.Value.Trim();
            if (!Uri.TryCreate(url, UriKind.Absolute, out var providerUri)
                || (providerUri.Scheme != Uri.UriSchemeHttp && providerUri.Scheme != Uri.UriSchemeHttps))
            {
                _logger.LogError("Gift card provider URL (key {Key}) for client {Client} is not a valid absolute http(s) URL",
                    GiftCardProviderConstants.VerifyUrlKey, client);
                response.Status = ResponseStatus.InvalidPropertyValues;
                response.ErrorMessage = $"Gift card provider URL ('{GiftCardProviderConstants.VerifyUrlKey}') configured for client '{client}' is invalid.";
                return response;
            }

            // 3. Call the external provider
            try
            {
                var externalCard = await _giftCardApiService.GetGiftCardAsync(url, id, cancellationToken);
                if (externalCard is null)
                {
                    response.Status = ResponseStatus.NotFound;
                    response.ErrorMessage = $"No gift card found for client '{client}' and id '{id}'.";
                    return response;
                }

                // 4. Map and return
                response.Result = externalCard.ToGiftCardDetailDto();
                return response;
            }
            catch (ExternalApiException ex)
            {
                _logger.LogError("Gift card provider {Host} returned {StatusCode} for client {Client}",
                    providerUri.Host, (int)ex.StatusCode, client);
                return BadGateway(response);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Gift card provider {Host} unreachable for client {Client}", providerUri.Host, client);
                return BadGateway(response);
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "Gift card provider {Host} timed out for client {Client}", providerUri.Host, client);
                return BadGateway(response);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Gift card provider {Host} returned an invalid JSON response for client {Client}", providerUri.Host, client);
                return BadGateway(response);
            }
        }

        private static ServiceResponse<GiftCardDetailDto> BadGateway(ServiceResponse<GiftCardDetailDto> response)
        {
            response.Status = ResponseStatus.BadGateway;
            response.ErrorMessage = ProviderErrorMessage;
            return response;
        }
    }
}
