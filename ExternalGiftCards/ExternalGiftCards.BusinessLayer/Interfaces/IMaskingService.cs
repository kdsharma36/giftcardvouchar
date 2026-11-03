namespace ExternalGiftCards.BusinessLayer.Interfaces
{
    /// <summary>
    /// Masks sensitive values (voucher ids, API keys, reference numbers) before they are
    /// written to logs or returned to callers.
    /// </summary>
    public interface IMaskingService
    {
        /// <summary>
        /// Replaces the last 4 characters of <paramref name="value"/> with "****".
        /// <list type="bullet">
        /// <item>"1234567890" -> "123456****"</item>
        /// <item>Strings of 4 characters or fewer -> "****" (nothing is left unmasked).</item>
        /// <item>null / empty -> returned unchanged.</item>
        /// </list>
        /// </summary>
        string? MaskLastFour(string? value);
    }
}
