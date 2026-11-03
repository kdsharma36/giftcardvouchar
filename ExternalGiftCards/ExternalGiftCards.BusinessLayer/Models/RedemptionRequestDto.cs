using System.Text.Json.Serialization;

namespace ExternalGiftCards.BusinessLayer.Models
{
    /// <summary>
    /// Request body of POST giftcard/redemption.
    /// Properties are nullable so missing fields reach the service validation
    /// (which returns the messagecode/errormessage contract) instead of the default ProblemDetails.
    /// </summary>
    public class RedemptionRequestDto
    {
        /// <summary>Gift card (voucher) identifier.</summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        /// <summary>Amount to redeem. Must be greater than 0.</summary>
        [JsonPropertyName("amount")]
        public decimal? Amount { get; set; }

        /// <summary>3-letter ISO 4217 currency code, e.g. EUR.</summary>
        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        /// <summary>Caller's unique reference number for this redemption.</summary>
        [JsonPropertyName("referencenumber")]
        public string? ReferenceNumber { get; set; }
    }
}
