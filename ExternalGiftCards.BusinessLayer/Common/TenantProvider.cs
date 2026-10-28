namespace ExternalGiftCards.BusinessLayer.Common
{
    public class TenantProvider : ITenantProvider
    {
        private TenantInfo? _tenantInfo;
        public TenantInfo? CurrentTenant => _tenantInfo;

        public void SetTenant(string tenantId, string connectionString)
        {
            _tenantInfo = new TenantInfo
            {
                TenantId = tenantId,
                ConnectionString = connectionString
            };
        }
    }
}
