using System.Text.Json;
using Keycloak.Auth.Api.Policies;
using Microsoft.AspNetCore.Authorization;

namespace Keycloak.Auth.Api.Handlers;

public class AdminFromPublicClientPolicyHandler : AuthorizationHandler<AdminFromPublicClientPolicy>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdminFromPublicClientPolicy requirement)
    {
        // 1. Проверка аутентификации
        if (!context.User.Identity?.IsAuthenticated ?? false)
        {
            return Task.CompletedTask;
        }

        // 2. Проверка клиента (azp)
        if (!context.User.HasClaim(c => c.Type == "azp" && c.Value == "public-client"))
        {
            return Task.CompletedTask;
        }

        // 3. Проверка роли admin в resource_access
        System.Security.Claims.Claim? resourceAccessClaim = context.User.FindFirst("resource_access");
        if (resourceAccessClaim == null)
        {
            return Task.CompletedTask;
        }

        try
        {
            JsonElement resourceAccess = JsonSerializer.Deserialize<JsonElement>(resourceAccessClaim.Value);
            
            if (resourceAccess.TryGetProperty("public-client", out JsonElement publicClient) &&
                publicClient.TryGetProperty("roles", out JsonElement roles))
            {
                foreach (JsonElement role in roles.EnumerateArray())
                {
                    if (role.ValueEquals("admin"))
                    {
                        context.Succeed(requirement);
                        return Task.CompletedTask;
                    }
                }
            }
        }
        catch (JsonException)
        {
            // Логирование ошибки при необходимости
        }

        return Task.CompletedTask;
    }
}
