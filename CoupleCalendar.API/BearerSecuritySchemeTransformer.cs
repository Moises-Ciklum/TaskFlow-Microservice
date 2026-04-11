using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace CoupleCalendar.API;

public class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        // 1. Creamos el esquema de seguridad
        var bearerScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            In = ParameterLocation.Header,
            BearerFormat = "JWT"
        };

        // 2. Lo guardamos en los componentes
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = bearerScheme;

        // 3. Creamos el requerimiento usando la NUEVA clase de referencia
        var requirement = new OpenApiSecurityRequirement();
        var reference = new OpenApiSecuritySchemeReference("Bearer", null, null);
        requirement.Add(reference, new List<string>());

        // 4. Lo aplicamos a la propiedad global
        document.Security ??= new List<OpenApiSecurityRequirement>();
        document.Security.Add(requirement);

        return Task.CompletedTask;
    }
}