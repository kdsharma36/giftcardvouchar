using ExternalGiftCards.BusinessLayer.Common;
using ExternalGiftCards.BusinessLayer.Interfaces;
using ExternalGiftCards.BusinessLayer.MappingProfile;
using ExternalGiftCards.BusinessLayer.Models;
using ExternalGiftCards.BusinessLayer.Models.External;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ExternalGiftCards.BusinessLayer.Services
{
    public class GiftCardService : IGiftCardService
    {
        private const string ProviderErrorMessage = "The external gift card provider could not be reached or returned an error.";

        private readonly IPServerService _pServerService;
        private readonly IGiftCardExternalApiService _giftCardApiService;
        private readonly ILogger<GiftCardService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMaskingService _maskingService;

        public GiftCardService(IPServerService pServerService, IGiftCardExternalApiService giftCardApiService, ILogger<GiftCardService> logger, IHttpContextAccessor httpContextAccessor, IMaskingService maskingService)
        {
            _pServerService = pServerService;
            _giftCardApiService = giftCardApiService;
            _logger = logger;
            _maskingService = maskingService;
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
            url = url.Replace("{id}", id);
           
            if (!Uri.TryCreate(url, UriKind.Absolute, out var providerUri)
                || (providerUri.Scheme != Uri.UriSchemeHttp && providerUri.Scheme != Uri.UriSchemeHttps))
            {
                _logger.LogError("Gift card provider URL (key {Key}) for client {Client} is not a valid absolute http(s) URL",
                    GiftCardProviderConstants.VerifyUrlKey, client);
                response.Status = ResponseStatus.InvalidPropertyValues;
                response.ErrorMessage = $"Gift card provider URL ('{GiftCardProviderConstants.VerifyUrlKey}') configured for client '{client}' is invalid.";
                return response;
            }

            // 2.1. Read the provider API key from p_server (key PRVKEY)
            var providerkey = await _pServerService.GetPServerAsync(
                client, GiftCardProviderConstants.PServerMc, GiftCardProviderConstants.providerAPIkey, cancellationToken);

            if (providerkey.Status != ResponseStatus.Ok || string.IsNullOrWhiteSpace(providerkey.Result?.Value))
            {
                _logger.LogWarning("Gift card provider API key (key {Key}) not configured for client {Client}, mc {Mc}",
                    GiftCardProviderConstants.providerAPIkey, client, GiftCardProviderConstants.PServerMc);
                response.Status = ResponseStatus.NotFound;
                response.ErrorMessage = $"Gift card provider API key ('{GiftCardProviderConstants.providerAPIkey}') is not configured for client '{client}'.";
                return response;
            }

            var apikey = providerkey.Result.Value.Trim();
            
            // 3. Call the external provider
            try
            {
                var externalCard = await _giftCardApiService.GetGiftCardAsync(apikey, url, cancellationToken);
                if (externalCard is null)
                {
                    response.Status = ResponseStatus.NotFound;
                    response.ErrorMessage = $"No gift card found for client '{client}' and id '{_maskingService.MaskLastFour(id)}'.";
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

        public async Task<IServiceResponse<RedemptionResponseDto>> RedeemGiftCardAsync(string client, RedemptionRequestDto request, CancellationToken cancellationToken = default)
        {
            var response = new ServiceResponse<RedemptionResponseDto>();

            // 1. Validate input
            var validationError = ValidateRedemptionRequest(client, request);
            if (validationError is not null)
            {
                return Fail(response, ResponseStatus.BadRequest, validationError.Value.Code, validationError.Value.Message);
            }

            client = client.Trim();
            var redemption = new RedemptionRequestDto
            {
                Id = request.Id!.Trim(),
                Amount = request.Amount,
                Currency = request.Currency!.Trim().ToUpperInvariant(),
                ReferenceNumber = request.ReferenceNumber!.Trim()
            };

            // 2. Read the provider redemption URL from p_server (key REDEEM)
            var pServer = await _pServerService.GetPServerAsync(
                client, GiftCardProviderConstants.PServerMc, GiftCardProviderConstants.RedeemUrlKey, cancellationToken);

            if (pServer.Status != ResponseStatus.Ok || string.IsNullOrWhiteSpace(pServer.Result?.Value))
            {
                _logger.LogError("Gift card provider redemption URL (key {Key}) not configured for client {Client}, mc {Mc}",
                    GiftCardProviderConstants.RedeemUrlKey, client, GiftCardProviderConstants.PServerMc);
                return UnexpectedError(response);
            }

            var url = pServer.Result.Value.Trim();     
            

            if (!Uri.TryCreate(url, UriKind.Absolute, out var providerUri)
                || (providerUri.Scheme != Uri.UriSchemeHttp && providerUri.Scheme != Uri.UriSchemeHttps))
            {
                _logger.LogError("Gift card provider redemption URL (key {Key}) for client {Client} is not a valid absolute http(s) URL",
                    GiftCardProviderConstants.RedeemUrlKey, client);
                return UnexpectedError(response);
            }

            // 2.1. Read the provider API key from p_server (key PRVKEY)
            var providerKey = await _pServerService.GetPServerAsync(
                client, GiftCardProviderConstants.PServerMc, GiftCardProviderConstants.providerAPIkey, cancellationToken);

            if (providerKey.Status != ResponseStatus.Ok || string.IsNullOrWhiteSpace(providerKey.Result?.Value))
            {
                _logger.LogError("Gift card provider API key (key {Key}) not configured for client {Client}, mc {Mc}",
                    GiftCardProviderConstants.providerAPIkey, client, GiftCardProviderConstants.PServerMc);
                return UnexpectedError(response);
            }

            var apiKey = providerKey.Result.Value.Trim();

            // 3. Call the external provider (no retry - POST is not idempotent)
            try
            {
                var result = await _giftCardApiService.RedeemGiftCardAsync(
                    apiKey, url, redemption.ToExternalRedemptionRequest(), cancellationToken);

                switch (result.Outcome)
                {
                    case ExternalRedemptionOutcome.Success:
                        if (result.Response is null)
                        {
                            throw new JsonException("The provider returned an empty redemption response.");
                        }

                        // 4. Map and return
                        response.Result = result.Response.ToRedemptionResponseDto(redemption);
                        _logger.LogInformation("Gift card redeemed for client {Client}, reference {ReferenceNumber}, transaction {TransactionId}",
                            client, redemption.ReferenceNumber, response.Result.TransactionId);
                        return response;

                    case ExternalRedemptionOutcome.Rejected:
                        _logger.LogWarning("Gift card provider {Host} rejected redemption for client {Client}, reference {ReferenceNumber} with code {MessageCode}",
                            providerUri.Host, client, redemption.ReferenceNumber, result.Error!.MessageCode);
                        return Fail(response, ResponseStatus.BadRequest, result.Error.MessageCode,
                            string.IsNullOrWhiteSpace(result.Error.ErrorMessage)
                                ? DefaultErrorMessage(result.Error.MessageCode, redemption.ReferenceNumber)
                                : result.Error.ErrorMessage);

                    case ExternalRedemptionOutcome.Unauthorized:
                        _logger.LogWarning("Gift card provider {Host} rejected the API key for client {Client}, reference {ReferenceNumber}",
                            providerUri.Host, client, redemption.ReferenceNumber);
                        return Fail(response, ResponseStatus.Unauthorized, GiftCardErrorCodes.Unauthorized, GiftCardErrorCodes.UnauthorizedMessage);

                    case ExternalRedemptionOutcome.NotFound:
                        _logger.LogWarning("Gift card provider {Host} did not find the voucher for client {Client}, reference {ReferenceNumber}",
                            providerUri.Host, client, redemption.ReferenceNumber);
                        return Fail(response, ResponseStatus.BadRequest, GiftCardErrorCodes.InvalidVoucherId, GiftCardErrorCodes.InvalidVoucherIdMessage);

                    default:
                        return UnexpectedError(response);
                }
            }
            catch (ExternalApiException ex)
            {
                _logger.LogError("Gift card provider {Host} returned {StatusCode} on redemption for client {Client}, reference {ReferenceNumber}",
                    providerUri.Host, (int)ex.StatusCode, client, redemption.ReferenceNumber);
                return UnexpectedError(response);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Gift card provider {Host} unreachable on redemption for client {Client}, reference {ReferenceNumber}",
                    providerUri.Host, client, redemption.ReferenceNumber);
                return UnexpectedError(response);
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "Gift card provider {Host} timed out on redemption for client {Client}, reference {ReferenceNumber}",
                    providerUri.Host, client, redemption.ReferenceNumber);
                return UnexpectedError(response);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Gift card provider {Host} returned an invalid redemption response for client {Client}, reference {ReferenceNumber}",
                    providerUri.Host, client, redemption.ReferenceNumber);
                return UnexpectedError(response);
            }
        }

        private static (string Code, string Message)? ValidateRedemptionRequest(string client, RedemptionRequestDto? request)
        {
            if (request is null)
            {
                return (GiftCardErrorCodes.InvalidRequest, "Request body is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Id))
            {
                return (GiftCardErrorCodes.InvalidVoucherId, GiftCardErrorCodes.InvalidVoucherIdMessage);
            }

            if (string.IsNullOrWhiteSpace(client))
            {
                return (GiftCardErrorCodes.InvalidRequest, "'client' is required.");
            }

            if (request.Amount is null || request.Amount <= 0)
            {
                return (GiftCardErrorCodes.InvalidRequest, "'amount' is required and must be greater than 0.");
            }

            var currency = request.Currency?.Trim();
            if (string.IsNullOrEmpty(currency) || currency.Length != 3 || currency != "NOK")
            {
                return (GiftCardErrorCodes.InvalidRequest, "'currency' is required and must be a NOK.");
            }

            if (string.IsNullOrWhiteSpace(request.ReferenceNumber))
            {
                return (GiftCardErrorCodes.InvalidRequest, "'referencenumber' is required.");
            }

            return null;
        }

        private static string DefaultErrorMessage(string code, string? referenceNumber) => code switch
        {
            GiftCardErrorCodes.InvalidVoucherId => GiftCardErrorCodes.InvalidVoucherIdMessage,
            GiftCardErrorCodes.ReferenceAlreadyProcessed => GiftCardErrorCodes.ReferenceAlreadyProcessedMessage(referenceNumber ?? string.Empty),
            GiftCardErrorCodes.RedemptionNotFound => GiftCardErrorCodes.RedemptionNotFoundMessage(referenceNumber ?? string.Empty),
            _ => GiftCardErrorCodes.InvalidRequestMessage
        };

        private static ServiceResponse<T> Fail<T>(ServiceResponse<T> response, ResponseStatus status, string code, string message)
        {
            response.Status = status;
            response.ErrorCode = code;
            response.ErrorMessage = message;
            return response;
        }

        // Any status other than Ok / BadRequest / Unauthorized is returned as 500 / code 100 by ErrorCodeResultMapper.
        private static ServiceResponse<T> UnexpectedError<T>(ServiceResponse<T> response) =>
            Fail(response, ResponseStatus.BadGateway, GiftCardErrorCodes.UnexpectedError, GiftCardErrorCodes.UnexpectedErrorMessage);

        private static ServiceResponse<GiftCardDetailDto> BadGateway(ServiceResponse<GiftCardDetailDto> response)
        {
            response.Status = ResponseStatus.BadGateway;
            response.ErrorMessage = ProviderErrorMessage;
            return response;
        }
    }
}
