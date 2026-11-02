namespace ExternalGiftCards.BusinessLayer.Models.External
{
    public enum ExternalRedemptionOutcome
    {
        /// <summary>2xx - <see cref="ExternalRedemptionResult.Response"/> is set.</summary>
        Success,

        /// <summary>400 with a messagecode/errormessage body - <see cref="ExternalRedemptionResult.Error"/> is set.</summary>
        Rejected,

        /// <summary>401 / 403 - provider rejected the API key.</summary>
        Unauthorized,

        /// <summary>404 - voucher not found at the provider.</summary>
        NotFound
    }

    /// <summary>
    /// Result of the external provider's redemption call. Any other provider failure is thrown
    /// as <see cref="Common.ExternalApiException"/>.
    /// </summary>
    public class ExternalRedemptionResult
    {
        public ExternalRedemptionOutcome Outcome { get; init; }
        public ExternalRedemptionResponse? Response { get; init; }
        public ExternalErrorResponse? Error { get; init; }
    }
}
