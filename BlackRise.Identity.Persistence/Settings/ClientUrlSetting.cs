namespace BlackRise.Identity.Persistence.Settings;

public class ClientUrlSetting
{
    public string EmailConfirmation { get; set; }
    public string ResetPassword { get; set; }
    public string Login { get; set; }
    public string SenderUrl { get; set; }
    public string EmailConfirmationTokenExpire { get; set; }
    public string ResetPasswordTokenExpire { get; set; }
}
