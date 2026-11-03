using ExternalGiftCards.BusinessLayer.Models;

namespace ExternalGiftCards.BusinessLayer.Common
{
    public class TenantContext
    {
        public string TenantId { get; set; } = string.Empty;

        public TenantConfiguration TenantConfiguration { get; set; } = new();
    }
}
