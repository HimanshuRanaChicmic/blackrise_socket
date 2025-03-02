using BlackRise.Identity.Application.Settings;
using MediatR;
using Microsoft.Extensions.Options;
using System.Web;

namespace BlackRise.Identity.Application.Feature.Login.Queries.LinkedIn;

public class GetLinkedInQueryHandler : IRequestHandler<GetLinkedInQuery, string>
{
    private readonly LinkedInSetting _linkedInSetting;

    public GetLinkedInQueryHandler(IOptions<LinkedInSetting> linkedInSetting)
    {
        _linkedInSetting = linkedInSetting.Value;
    }

    public async Task<string> Handle(GetLinkedInQuery request, CancellationToken cancellationToken)
    {
        string clientId = _linkedInSetting.LinkedInClientId;
        string redirectUri = _linkedInSetting.LinkedInRedirectUri;
        string state = Guid.NewGuid().ToString(); // Optional: Use this to prevent CSRF attacks
        string scope = "r_liteprofile r_emailaddress";
       
        string encodedScope = HttpUtility.UrlEncode(scope); // Encodes space as %20
        string encodedRedirectUri = HttpUtility.UrlEncode(redirectUri);

        string linkedInAuthUrl = $"https://www.linkedin.com/oauth/v2/authorization?response_type=code&client_id={clientId}&redirect_uri={encodedRedirectUri}&scope={encodedScope}&state={state}";

        return linkedInAuthUrl;
    }
}
