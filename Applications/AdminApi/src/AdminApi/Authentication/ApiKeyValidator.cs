using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Authentication;

public class ApiKeyValidator
{
    private readonly ApiKeyAuthenticationSchemeOptions _options;

    public ApiKeyValidator(IOptionsMonitor<ApiKeyAuthenticationSchemeOptions> options)
    {
        _options = options.Get("ApiKey");
    }

    public bool IsApiKeyValid(string? apiKey)
    {
        var apiKeyIsConfigured = !string.IsNullOrEmpty(_options.ApiKey);

        if (!apiKeyIsConfigured) return true;

        return apiKey == _options.ApiKey;
    }
}
