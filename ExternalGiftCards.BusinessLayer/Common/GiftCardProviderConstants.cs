namespace ExternalGiftCards.BusinessLayer.Common
{
    /// <summary>
    /// p_server / p_external_provider lookup values for the external gift card provider.
    /// </summary>
    public static class GiftCardProviderConstants
    {
        /// <summary>
        /// p_server.mc of the external gift card provider entry.
        /// TODO: replace with the real mc. Until then the lookup finds no row and the endpoint returns 404.
        /// </summary>
        public const string PServerMc = "TODO_SET_MC";

        /// <summary>
        /// p_external_provider.key holding the provider's gift card detail URL.
        /// TODO: confirm spelling as stored in the database (VERFIY vs VERIFY).
        /// </summary>
        public const string VerifyUrlKey = "VERFIY";
    }
}
