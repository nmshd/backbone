namespace SpecFlowChallenges.Specs.Models;
public class RequestConfiguration
{
    public AccessTokenResponse? TokenResponse { get; set; }
    public string ContentType { get; set; }
    public string AcceptHeader { get; set; }
    public string Body { get; set; }
}
