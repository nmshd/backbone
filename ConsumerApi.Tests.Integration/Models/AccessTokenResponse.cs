using System.Text.Json.Serialization;

namespace Backbone.ConsumerApi.Tests.Integration.Models;

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
        ExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn);
    }

    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    public DateTime ExpiresAt { get; }
    public bool IsExpired => (ExpiresAt - DateTime.UtcNow).TotalSeconds <= 15;
}
