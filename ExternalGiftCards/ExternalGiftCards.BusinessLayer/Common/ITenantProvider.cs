namespace ExternalGiftCards.BusinessLayer.Common
{
    public interface ITenantProvider
    {
        TenantInfo? CurrentTenant { get; }
        void SetTenant(string tenantId, string connectionString);
    }
}
