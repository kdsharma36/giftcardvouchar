using System.Text.Json.Serialization;

namespace ExternalGiftCards.BusinessLayer.Models
{
    /// <summary>
    /// Error body returned by the redemption endpoint for every non-200 response.
    /// </summary>
    public class ErrorResponseDto
    {
        [JsonPropertyName("messagecode")]
        public string MessageCode { get; set; } = string.Empty;

        [JsonPropertyName("errormessage")]
        public string ErrorMessage { get; set; } = string.Empty;

        public ErrorResponseDto()
        {
        }

        public ErrorResponseDto(string messageCode, string errorMessage)
        {
            MessageCode = messageCode;
            ErrorMessage = errorMessage;
        }
    }
}
