using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ExternalGiftCards.ServiceExtension
{
    public static class SwaggerExtention
    {
        public static void AddSwaggerService(this IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo { Title = "ExternalGiftCards", Version = "v1" });
                s.DocumentFilter<SwaggerAPIDocsShorting>();
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    s.IncludeXmlComments(xmlPath);
                }
            });
        }
    }

    /// <summary>
    /// Orders the Swagger paths alphabetically.
    /// </summary>
    public class SwaggerAPIDocsShorting : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var ordered = swaggerDoc.Paths.OrderBy(p => p.Key).ToList();
            swaggerDoc.Paths.Clear();
            foreach (var path in ordered)
            {
                swaggerDoc.Paths.Add(path.Key, path.Value);
            }
        }
    }
}
