using Microsoft.AspNetCore.Authorization;

namespace Keycloak.Auth.Api.Policies;

public class AdminFromPublicClientPolicy : IAuthorizationRequirement
{
    public const string PolicyName = "AdminFromPublicClientPolicy";
}
