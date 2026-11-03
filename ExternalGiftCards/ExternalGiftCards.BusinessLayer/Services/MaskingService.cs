using ExternalGiftCards.BusinessLayer.Interfaces;

namespace ExternalGiftCards.BusinessLayer.Services
{
    /// <summary>
    /// Stateless implementation of <see cref="IMaskingService"/>. Safe to register as a singleton.
    /// </summary>
    public class MaskingService : IMaskingService
    {
        private const int MaskLength = 4;
        private const string Mask = "****";

        public string? MaskLastFour(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            // Short values: mask everything so no part of the secret leaks.
            if (value.Length <= MaskLength)
            {
                return Mask;
            }

            return string.Concat(value.AsSpan(0, value.Length - MaskLength), Mask);
        }
    }
}
