using Keycloak.Auth.Api.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Keycloak.Auth.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController
{
    private const string AdminGreeting = "Hello admin!";

    private readonly IHttpContextAccessor _context;

    public UserController(IHttpContextAccessor context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet("claims")]
    public IDictionary<string, string> GetClaims()
    {
        return _context.HttpContext?.User.Claims.ToDictionary(c => c.Type, c => c.Value);
    }

    [Authorize(Policy = "AdminFromPublicClientPolicy")]
    [HttpGet("greeting-by-policy")]
    public string GetGreetingByPolicy()
    {
        return AdminGreeting;
    }
    
    [CheckPublicClientRole(roleName:"admin")]
    [HttpGet("greeting-by-attribute")]
    public string GetGreetingByAttribute()
    {
        return AdminGreeting;
    }
}
