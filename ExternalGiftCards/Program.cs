using Inhouse.Platform.AspNetCore;
using Inhouse.Platform.AspNetCore.Extensions;
using Inhouse.Platform.Observability.AspNetCore;
using Inhouse.Platform.Observability.AspNetCore.Extensions;
using Inhouse.Platform.SqlServer;
using Inhouse.Platform.SqlServer.SimpleInjector;
using SimpleInjector;
using ExternalGiftCards.BusinessLayer;
using ExternalGiftCards.BusinessLayer.Models;
using ExternalGiftCards.Middleware;
using ExternalGiftCards.ServiceExtension;

var basePath = Environment.GetEnvironmentVariable("BasePath") ?? "/external-gift-cards";

return await SimpleInjectorSqlServerBootstrap.MultiTenancySqlServerBootstrap(
    args,
    _ => { },
    ConfigureAppBuilder,
    ConfigureContainer,
    ConfigureApp);

void ConfigureAppBuilder(WebApplicationBuilder builder)
{
    builder.Services.RegisterDependency();
    builder.Services.AddControllers();
    builder.Services.AddHttpClient();
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerService();
    builder.Services.Configure<ExternalGiftCardsOptions>(builder.Configuration.GetSection("ExternalGiftCards"));
}

void ConfigureContainer(Container container)
{
    container.Options.EnableAutoVerification = false;
}

void ConfigureApp(WebApplication app)
{
    app.UseObservability();
    app.UsePathBaseFromConfiguration("/" + basePath);
    app.UseCors("AllowAll");
    if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/" + basePath.TrimStart('/').TrimEnd('/') + "/swagger/v1/swagger.json", "ExternalGiftCards API V1");
        });
    }

    app.UseRouting();
    app.Use(async (context, next) =>
    {
        await next();

        if (context.Response.StatusCode == 404 && !context.Response.HasStarted && (!context.Request.Path.ToString().ToLower().Contains("health")))
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\":\"Tenant not found\"}");
        }
    });
    app.UseTenantMiddleware(); // Must come before endpoints
    app.MapControllers();
    app.MapVersion();
    app.UseAuthorization();
}
