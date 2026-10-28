using ExternalGiftCards.BusinessLayer.Common;
using ExternalGiftCards.BusinessLayer.Interfaces;
using ExternalGiftCards.BusinessLayer.Services;
using ExternalGiftCards.DataAccessLayer;
using ExternalGiftCards.Repositories.Interfaces;
using ExternalGiftCards.Repositories.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ExternalGiftCards.BusinessLayer
{
    public static class ConfigureDependencies
    {
        public static void RegisterDependency(this IServiceCollection services)
        {
            // ------------------------------------------------------------------
            //  Services (BusinessLayer/Services)
            //  services.AddScoped<IXService, XService>();
            //  services.AddHttpClient<IXApiService, XApiService>("xApi");
            //
            //  Repositories (Repositories/Repositories)
            //  services.AddScoped<IXRepository, XRepository>();
            // ------------------------------------------------------------------

            services.AddScoped<ITenantProvider, TenantProvider>();
            services.AddScoped<TenantContext>();
            services.AddScoped<ApplicationDbContext>(provider =>
            {
                var tenant = provider.GetRequiredService<ITenantProvider>().CurrentTenant;
                if (tenant == null || string.IsNullOrWhiteSpace(tenant.ConnectionString))
                {
                    return null!;
                }

                return new ApplicationDbContext(tenant.ConnectionString);
            });

            // EF Core context - connection string resolved per request from the current tenant
            services.AddDbContext<GiftCardDbContext>((provider, options) =>
            {
                var tenant = provider.GetRequiredService<ITenantProvider>().CurrentTenant;
                if (tenant == null || string.IsNullOrWhiteSpace(tenant.ConnectionString))
                {
                    options.UseSqlServer(); // no tenant -> fails on first query with a clear EF error
                    return;
                }

                options.UseSqlServer(tenant.ConnectionString);
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            // Repositories
            services.AddScoped<IPServerRepository, PServerRepository>();

            // Services
            services.AddScoped<IPServerService, PServerService>();
        }
    }
}
