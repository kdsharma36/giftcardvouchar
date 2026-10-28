namespace ExternalGiftCards.BusinessLayer.Models
{
    /// <summary>
    /// Bound from the "ExternalGiftCards" configuration section.
    /// </summary>
    public class ExternalGiftCardsOptions
    {
        public Dictionary<string, TenantConfiguration> Tenants { get; set; } = new();
    }

    /// <summary>
    /// Per-tenant settings (placeholder - add properties as features require them).
    /// </summary>
    public class TenantConfiguration
    {
    }
}
