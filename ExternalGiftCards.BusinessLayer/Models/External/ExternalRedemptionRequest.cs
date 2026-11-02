using System.Text.Json.Serialization;

namespace ExternalGiftCards.BusinessLayer.Models.External
{
    /// <summary>
    /// Body sent to the external provider's redemption endpoint (p_server key REDEEM).
    /// </summary>
    public class ExternalRedemptionRequest
    {
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
