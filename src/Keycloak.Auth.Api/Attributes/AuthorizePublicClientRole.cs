using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Keycloak.Auth.Api.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public sealed class CheckPublicClientRoleAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string _roleName;

    public CheckPublicClientRoleAttribute(string roleName)
    {
        _roleName = roleName;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // 1. Проверяем, аутентифицирован ли пользователь
        if (context.HttpContext.User.Identity is { IsAuthenticated: false })
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // 2. Проверяем наличие роли

        bool hasRole = false;
        if (context.HttpContext.User.HasClaim(c => c.Type == "resource_access"))
        {
            Claim? resourceAccessClaim = context.HttpContext.User.FindFirst("resource_access");
            JsonElement resourceAccess = JsonSerializer.Deserialize<JsonElement>(resourceAccessClaim!.Value);

            if (resourceAccess.TryGetProperty("public-client", out JsonElement publicClient))
            {
                hasRole = publicClient.TryGetProperty("roles", out JsonElement roles) &&
                       roles.EnumerateArray().Any(r => r.GetString() == _roleName);
            }
        }
       
        if (!hasRole)
        {
            context.Result = new ForbidResult(); // 403
        }
    }
}
