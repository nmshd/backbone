using Devices.API.Models;
using IdentityServer4.Models;

namespace Devices.API;

public class Config
{
    /*******************************************************
     * Difference between Apis and Resources (source: https://brockallen.com/2019/02/25/scope-and-claims-design-in-identityserver/):
     * APIs = Scopes in terms of OAuth2.0 (= APIs that should be accessible), optionally contains Claims that should added when the API is requested, Example: new ApiResource("calendar", new[] { "country" }),
     * Resources: Scopes in terms of OpenID Connect (= sets of Claims), Example: new IdentityResource("employee_info", new[] {"employee_id", "building_number", "office_number"})
     *******************************************************/

    public static IEnumerable<ApiScope> GetApiScopes()
    {
        return new List<ApiScope>
        {
            new(CustomScopes.Apis.MESSAGES, "Messages"),
            new(CustomScopes.Apis.CHALLENGES, "Challenges"),
            new(CustomScopes.Apis.DEVICES, "Devices"),
            new(CustomScopes.Apis.SYNCHRONIZATION, "Synchronization"),
            new(CustomScopes.Apis.TOKENS, "Tokens"),
            new(CustomScopes.Apis.FILES, "Files"),
            new(CustomScopes.Apis.RELATIONSHIPS, "Relationships")
        };
    }

    public static IEnumerable<ApiResource> GetApiResources()
    {
        return new List<ApiResource>
        {
            new(CustomScopes.Apis.MESSAGES, "Messages API")
            {
                Scopes = {CustomScopes.Apis.MESSAGES}
            },
            new(CustomScopes.Apis.CHALLENGES, "Challenges API")
            {
                Scopes = {CustomScopes.Apis.CHALLENGES}
            },
            new(CustomScopes.Apis.DEVICES, "Devices API")
            {
                Scopes = {CustomScopes.Apis.DEVICES}
            },
            new(CustomScopes.Apis.SYNCHRONIZATION, "Synchronization API")
            {
                Scopes = {CustomScopes.Apis.SYNCHRONIZATION}
            },
            new(CustomScopes.Apis.TOKENS, "Tokens API")
            {
                Scopes = {CustomScopes.Apis.TOKENS}
            },
            new(CustomScopes.Apis.FILES, "Files API")
            {
                Scopes = {CustomScopes.Apis.FILES}
            },
            new(CustomScopes.Apis.RELATIONSHIPS, "Relationships API")
            {
                Scopes = {CustomScopes.Apis.RELATIONSHIPS}
            }
        };
    }

    public static IEnumerable<IdentityResource> GetIdentitityResources()
    {
        return new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new(CustomScopes.IdentityResources.IDENTITY_INFORMATION, "Identity Information", new[]
            {
                CustomClaims.ADDRESS
            }),
            new(CustomScopes.IdentityResources.DEVICE_INFORMATION, "Device Information", new[]
            {
                CustomClaims.DEVICE_ID
            })
        };
    }

    public static IEnumerable<Client> GetClients()
    {
        return new List<Client>();
    }
}
