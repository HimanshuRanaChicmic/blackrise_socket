namespace BlackRise.Identity.Persistence.Settings;

public class JwtSetting 
{
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int DurationInMinutes { get; set; }
}
