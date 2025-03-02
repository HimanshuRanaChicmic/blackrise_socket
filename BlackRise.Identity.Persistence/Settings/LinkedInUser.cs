using System.Text.Json.Serialization;

namespace BlackRise.Identity.Persistence.Settings
{
    public class LinkedInUser
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("localizedFirstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("localizedLastName")]
        public string LastName { get; set; }

        [JsonPropertyName("emailAddress")]
        public string Email { get; set; }
    }
}
