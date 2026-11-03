using ExternalGiftCards.BusinessLayer.Common;
using ExternalGiftCards.BusinessLayer.Models;
using Inhouse.Platform.SqlServer;
using Microsoft.Extensions.Options;

namespace ExternalGiftCards.Middleware
{
    /// <summary>
    /// Resolves the tenant from the {tenantId} route value, builds its SQL Server connection string
    /// and populates the scoped <see cref="ITenantProvider"/> and <see cref="TenantContext"/>.
    /// </summary>
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ExternalGiftCardsOptions _options;

        public TenantMiddleware(RequestDelegate next, IConfiguration configuration, IOptions<ExternalGiftCardsOptions> options)
        {
            _next = next;
            _configuration = configuration;
            _options = options.Value;
        }

        public async Task Invoke(HttpContext context, ITenantProvider tenantProvider, TenantContext tenantContext)
        {
            var tenantId = context.Request.RouteValues["tenantId"]?.ToString();

            if (!string.IsNullOrEmpty(tenantId))
            {
                try
                {
                    var connStr = ConnectionStringBuilder.BuildConnectionString(_configuration, tenantId);
                    if (!string.IsNullOrEmpty(connStr))
                    {
                        tenantProvider.SetTenant(tenantId, connStr);
                        tenantContext.TenantId = tenantId;
                        if (_options.Tenants.TryGetValue(tenantId, out var tenantConfig))
                        {
                            tenantContext.TenantConfiguration = tenantConfig;
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync($"Configuration section for tenant with id '{tenantId}' not found.");
                        return;
                    }
                }
                catch (InvalidOperationException ioe)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync(ioe.Message);
                    return;
                }
                catch (Exception)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync($"Configuration section for tenant with id '{tenantId}' not found.");
                    return;
                }
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Extension method used to add the tenant middleware to the HTTP request pipeline.
    /// </summary>
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseTenantMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TenantMiddleware>();
        }
    }
}
