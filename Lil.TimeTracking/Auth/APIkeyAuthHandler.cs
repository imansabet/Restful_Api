using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace Lil.TimeTracking.Auth;

public class APIKeyOptions : AuthenticationSchemeOptions
{
    public string? DisplayMessage { get; set; }
}
public class APIKeyAuthHandler : AuthenticationHandler<APIKeyOptions>
{
    private string[] KEYS = { "123456789", "987654321" };
    public APIKeyAuthHandler(IOptionsMonitor<APIKeyOptions> options
        , ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
    { }
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        return Task.FromResult(AuthenticateResult.Fail("not implemented"));
    } 


}
