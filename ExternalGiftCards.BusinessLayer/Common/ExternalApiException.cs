using System.Net;

namespace ExternalGiftCards.BusinessLayer.Common
{
    /// <summary>
    /// Thrown when the external gift card provider returns a non-success status code (other than 404).
    /// </summary>
    public class ExternalApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        /// <summary>Response body from the provider, truncated. Not returned to callers.</summary>
        public string? ResponseBody { get; }

        public ExternalApiException(HttpStatusCode statusCode, string? responseBody)
            : base($"External gift card API returned {(int)statusCode} ({statusCode}).")
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
        }
    }
}
