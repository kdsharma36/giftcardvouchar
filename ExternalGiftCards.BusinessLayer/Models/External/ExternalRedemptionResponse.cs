using System.Text.Json.Serialization;

namespace ExternalGiftCards.BusinessLayer.Models.External
{
    /// <summary>
    /// Success response of the external provider's redemption endpoint.
    /// </summary>
    public class ExternalRedemptionResponse
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("transactionId")]
        public string? TransactionId { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("amount")]
        public decimal? Amount { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("referencenumber")]
        public string? ReferenceNumber { get; set; }
    }
}
