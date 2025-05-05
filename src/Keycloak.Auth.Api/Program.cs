using System.Security.Claims;
using System.Text.Json;
using Keycloak.Auth.Api.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminFromPublicClientPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("azp", "public-client"); // Проверяем клиента (azp = authorized party)
        policy.RequireAssertion(context =>
        {
            // Проверяем, есть ли роль admin в resource_access.public-client.roles
            if (context.User.HasClaim(c => c.Type == "resource_access"))
            {
                Claim? resourceAccessClaim = context.User.FindFirst("resource_access");
                JsonElement resourceAccess = JsonSerializer.Deserialize<JsonElement>(resourceAccessClaim!.Value);

                if (resourceAccess.TryGetProperty("public-client", out JsonElement publicClient))
                {
                    return publicClient.TryGetProperty("roles", out JsonElement roles) &&
                           roles.EnumerateArray().Any(r => r.GetString() == "admin");
                }
            }

            return false;
        });
    });
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RefreshOnIssuerKeyNotFound = true;
        o.RequireHttpsMetadata = false;
        o.Audience = builder.Configuration["Authentication:Audience"];
        o.MetadataAddress = builder.Configuration["Authentication:MetadataAddress"]!;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Authentication:ValidIssuer"],
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            RoleClaimType = "roles", // Keycloak использует "roles" в resource_access
            NameClaimType = "preferred_username"
        };
    });

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("Keycloak.Auth.Api"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();

        tracing.AddOtlpExporter();
    });

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("users/me", (ClaimsPrincipal claimsPrincipal) =>
{
    return claimsPrincipal.Claims.ToDictionary(c => c.Type, c => c.Value);
}).RequireAuthorization();

app.MapGet("/helloAdmin", () => "Hello admin!")
    .RequireAuthorization("AdminFromPublicClientPolicy");


app.UseAuthentication();
app.UseAuthorization();

await app.RunAsync().ConfigureAwait(false);
