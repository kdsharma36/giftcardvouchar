using System.Text.Json.Serialization;

namespace ExternalGiftCards.BusinessLayer.Models
{
    /// <summary>
    /// 200 OK body of POST giftcard/redemption.
    /// </summary>
    public class RedemptionResponseDto
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("transactionId")]
        public string? TransactionId { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonPropertyName("referencenumber")]
        public string ReferenceNumber { get; set; } = string.Empty;
    }
}
