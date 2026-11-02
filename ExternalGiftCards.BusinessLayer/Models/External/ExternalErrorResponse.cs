namespace ExternalGiftCards.BusinessLayer.Models.External
{
    /// <summary>
    /// Error body returned by the external provider (400): { "messagecode": "104", "errormessage": "..." }.
    /// </summary>
    public class ExternalErrorResponse
    {
        public string MessageCode { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }
}
