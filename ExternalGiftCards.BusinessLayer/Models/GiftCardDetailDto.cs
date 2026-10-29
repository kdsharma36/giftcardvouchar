using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExternalGiftCards.BusinessLayer.Models
{
    /// <summary>
    /// External gift card detail returned by GET giftcard.
    /// TODO: currently a pass-through of the provider's JSON (all properties serialized at the top level).
    /// Replace with typed properties and mask sensitive fields (PIN, card number) once the contract is agreed.
    /// </summary>
    public class GiftCardDetailDto
    {
        [JsonExtensionData]
        public Dictionary<string, JsonElement> Fields { get; set; } = new();
    }
}
