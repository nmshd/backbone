using System.Text.Json.Serialization;

namespace Challenges.API.Client.Models;
public class AccessTokenResponse
{
    public AccessTokenResponse(
        string accessToken,
        string tokenType,
        int expiresIn)
    {
        AccessToken = accessToken;
        TokenType = tokenType;
        ExpiresIn = expiresIn;
        Expires = DateTime.UtcNow.AddSeconds(expiresIn);
    }

    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    public DateTime Expires { get; }
    public bool Expired => (Expires - DateTime.UtcNow).TotalSeconds <= 0;
}
