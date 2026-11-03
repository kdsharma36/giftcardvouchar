namespace ExternalGiftCards.BusinessLayer.Common
{
    /// <summary>
    /// Error codes (messagecode) and default messages returned by the redemption endpoint.
    /// </summary>
    public static class GiftCardErrorCodes
    {
        /// <summary>500 - unexpected error.</summary>
        public const string UnexpectedError = "100";

        /// <summary>401 - caller not authenticated / not authorized.</summary>
        public const string Unauthorized = "101";

        /// <summary>400 - invalid voucher id.</summary>
        public const string InvalidVoucherId = "102";

        /// <summary>
        /// 400 - request invalid or missing mandatory fields (other than id).
        /// TODO: confirm code 103 with the business.
        /// </summary>
        public const string InvalidRequest = "103";

        /// <summary>400 - reference number already processed.</summary>
        public const string ReferenceAlreadyProcessed = "104";

        /// <summary>400 - redemption for reference number not found.</summary>
        public const string RedemptionNotFound = "105";

        public const string UnexpectedErrorMessage = "an unexpected error occurred while processing the request.";
        public const string UnauthorizedMessage = "Unauthorized.";
        public const string InvalidVoucherIdMessage = "Invalid voucher id.";
        public const string InvalidRequestMessage = "Invalid request.";

        public static string ReferenceAlreadyProcessedMessage(string referenceNumber) =>
            $"Reference number {referenceNumber} is already processed.";

        public static string RedemptionNotFoundMessage(string referenceNumber) =>
            $"Reference number {referenceNumber} redemption is not found.";
    }
}
