using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExternalGiftCards.BusinessLayer.Models.External
{
    /// <summary>
    /// Response of the external provider's gift card detail call.
    /// TODO: the provider contract is not known yet. Every JSON property is captured as-is in
    /// <see cref="Fields"/>. Replace with typed properties ([JsonPropertyName]) once a sample is available.
    /// </summary>
    public class ExternalGiftCardResponse
    {
        [JsonExtensionData]
        public Dictionary<string, JsonElement> Fields { get; set; } = new();
    }
}
