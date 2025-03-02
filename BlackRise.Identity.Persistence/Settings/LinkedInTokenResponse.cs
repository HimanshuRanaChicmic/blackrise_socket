using System.Text.Json.Serialization;

namespace BlackRise.Identity.Persistence.Settings
{
    public class LinkedInTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
