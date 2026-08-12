using System.Net.Http.Headers;
using System.Security.Claims;

namespace TodoListApp.WebApp.Services;

public class JwtAuthorizationHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly JwtTokenGenerator tokenGenerator;

    public JwtAuthorizationHandler(IHttpContextAccessor httpContextAccessor, JwtTokenGenerator tokenGenerator)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.tokenGenerator = tokenGenerator;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var user = this.httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = user?.Identity?.Name ?? string.Empty;

        if (!string.IsNullOrEmpty(userId))
        {
            var token = this.tokenGenerator.GenerateToken(userId, userName);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
