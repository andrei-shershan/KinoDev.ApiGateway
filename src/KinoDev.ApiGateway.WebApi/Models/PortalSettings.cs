namespace KinoDev.ApiGateway.WebApi.Models;

public class PortalSettings
{
    public string GithubLink { get; set; } = string.Empty;
    public string LinkedinLink { get; set; } = string.Empty;

    public string StripePK { get; set; } = string.Empty;
    public NotificationsSettings Notifications { get; set; } = new NotificationsSettings();
}

public class NotificationsSettings
{
    public string SignInNotification { get; set; } = string.Empty;

    public string TestCreditCard { get; set; } = string.Empty;
}