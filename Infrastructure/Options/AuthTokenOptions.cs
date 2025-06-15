namespace Infrastructure.Options;

public class AuthTokenOptions
{
    public string Audience { get; set; }
    public string Key { get; set; }
    public string Issuer { get; set; }
    public uint TokenDuration { get; set; }
}