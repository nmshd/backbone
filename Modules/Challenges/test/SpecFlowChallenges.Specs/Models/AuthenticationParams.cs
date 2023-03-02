namespace SpecFlowChallenges.Specs.Models;
public class AuthenticationParameters
{
    public Dictionary<string, string> Parameters { get; set; }

    public AuthenticationParameters()
    {
        Parameters = new Dictionary<string, string>();
    }
}
