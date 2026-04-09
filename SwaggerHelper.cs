using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace DigitalSalaryService
{
    public static class SwaggerHelper
    {
        public static void ConfigureSwaggerGen(SwaggerGenOptions options)
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Digital Salary Service API",
                Description = "All reponses are in generic response model."
            });
            options.AddSecurityDefinition("ApiKey Authorization", new OpenApiSecurityScheme
            {
                Description = @"Token for ApiKey authorization.",
                Name = "ApiKey",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ApiKey Authorization"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });


            options.AddSecurityDefinition("Bearer",
                new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter into field the word 'Bearer' following by space and JWT",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });


            OpenApiSecurityRequirement openApiSecurityRequirement = new OpenApiSecurityRequirement();

            openApiSecurityRequirement.Add(new OpenApiSecurityScheme()
            {
                Description = "Please enter Bearer Token",
                In = ParameterLocation.Header, // where to find apiKey, probably in a header
                Name = "Bearer", //header with bearer
                Type = SecuritySchemeType.ApiKey, // this value is always "apiKey"
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            }, new List<string>());

            openApiSecurityRequirement.Add(new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            }, new List<string>());

            options.AddSecurityRequirement(openApiSecurityRequirement);

        }

        public static void ConfigureSwagger(SwaggerOptions swaggerOptions)
        {
            swaggerOptions.RouteTemplate = "swagger/" + "{documentName}/swagger.json";
        }

        public static void ConfigureSwaggerUI(SwaggerUIOptions swaggerUIOptions)
        {
            string swaggerJsonBasePath = string.IsNullOrWhiteSpace(swaggerUIOptions.RoutePrefix) ? "." : "..";
            swaggerUIOptions.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "");
            swaggerUIOptions.RoutePrefix = "swagger";
        }

    }
}
