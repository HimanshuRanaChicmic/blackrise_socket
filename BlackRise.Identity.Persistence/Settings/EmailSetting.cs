namespace BlackRise.Identity.Persistence.Settings;

public class EmailSetting
{
    public string From { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public bool EnableSsl { get; set; }
}
