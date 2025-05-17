using Backbone.UnitTestTools.FluentAssertions.Extensions;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class WellKnownStepDefinitions
{
    private readonly HttpClientFactory _httpClientFactory;
    private string? _response;

    public WellKnownStepDefinitions(HttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    #region When

    [When("^the user sends a GET request to the /.well-known/assetlinks.json endpoint$")]
    public async Task WhenTheUserSendsAGETRequestToTheWellKnownAssetlinksJsonEndpoint()
    {
        var httpClient = _httpClientFactory.CreateClient();

        _response = await httpClient.GetStringAsync(".well-known/assetlinks.json");
    }

    [When("^the user sends a GET request to the /.well-known/apple-app-site-association endpoint$")]
    public async Task WhenTheUserSendsAGETRequestToTheWellKnownAppleAppSiteAssociationEndpoint()
    {
        var httpClient = _httpClientFactory.CreateClient();

        _response = await httpClient.GetStringAsync(".well-known/apple-app-site-association");
    }

    #endregion

    #region Then

    [Then("the response contains the package name and the SHA256 fingerprint for the configured Android app")]
    public void WhenTheResponseContainsThePackageNameAndTheSha256FingerprintForTheConfiguredAndroidApp()
    {
        _response.Should().BeEquivalentToJson("""
                                              [
                                                {
                                                  "relation": [
                                                    "delegate_permission/common.handle_all_urls"
                                                  ],
                                                  "target": {
                                                    "namespace": "android_app",
                                                    "package_name": "eu.enmeshed.app",
                                                    "sha256_cert_fingerprints": [
                                                      "0F:6F:48:FF:59:E7:B4:EC:52:4A:BB:E7:45:41:E7:38:5B:13:D1:0B:14:37:CF:C3:59:08:DF:22:F3:CA:70:38"
                                                    ]
                                                  }
                                                }
                                              ]
                                              """);
    }

    [Then("the response contains the app ID for the configured iOS app")]
    public void WhenTheResponseContainsTheAppIdForTheConfiguredIosApp()
    {
        _response.Should().BeEquivalentToJson("""
                                              {
                                                "applinks": {
                                                  "details": [
                                                    {
                                                      "appIDs": [
                                                        "R7Y6NESEQW.eu.enmeshed.app"
                                                      ],
                                                      "components": [
                                                        {
                                                          "/": "/r/*",
                                                          "comment": "Matches any URL whose path starts with /r/"
                                                        }
                                                      ]
                                                    }
                                                  ]
                                                }
                                              }
                                              """);
    }

    #endregion
}
