using System.Text.Json.Serialization;
using Backbone.ConsumerApi.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi.Controllers.Onboarding;

[Route(".well-known")]
public class WellKnownController : Controller
{
    private readonly ConsumerApiConfiguration.WellKnownEndpointsConfiguration? _configuration;

    public WellKnownController(IOptions<ConsumerApiConfiguration> configuration)
    {
        _configuration = configuration.Value.WellKnownEndpoints;
    }

    [HttpGet("apple-app-site-association")]
    public IActionResult AppleAppSiteAssociation()
    {
        return _configuration == null ? NotFound() : Json(new AppleAppSiteAssociation(_configuration.AppleAppSiteAssociations));
    }

    [HttpGet("assetlinks.json")]
    public IActionResult AndroidAssetLinks()
    {
        return _configuration == null
            ? NotFound()
            : Json(_configuration.AndroidAssetLinks.Select(a => new AndroidAssetLink(
                new AndroidAssetLink.TargetModel
                {
                    PackageName = a.PackageName,
                    Sha256CertFingerprints = a.Sha256CertFingerprints
                }
            )));
    }
}

public class AndroidAssetLink
{
    public AndroidAssetLink(TargetModel target)
    {
        Target = target;
    }

    [JsonPropertyName("relation")]
    [UsedImplicitly(Reason = "Objects of this class are serialized to JSON and thus the properties are required.")]
    public string[] Relation { get; set; } = ["delegate_permission/common.handle_all_urls"];

    [JsonPropertyName("target")]
    [UsedImplicitly(Reason = "Objects of this class are serialized to JSON and thus the properties are required.")]
    public TargetModel Target { get; set; }

    public class TargetModel
    {
        [JsonPropertyName("namespace")]
        [UsedImplicitly(Reason = "Objects of this class are serialized to JSON and thus the properties are required.")]
        public string Namespace { get; set; } = "android_app";

        [JsonPropertyName("package_name")]
        [UsedImplicitly(Reason = "Objects of this class are serialized to JSON and thus the properties are required.")]
        public required string PackageName { get; set; }

        [JsonPropertyName("sha256_cert_fingerprints")]
        [UsedImplicitly(Reason = "Objects of this class are serialized to JSON and thus the properties are required.")]
        public required string[] Sha256CertFingerprints { get; set; }
    }
}

public class AppleAppSiteAssociation
{
    public AppleAppSiteAssociation(string[]? appIds)
    {
        AppLinks = new AppLinksModel(appIds);
    }

    [JsonPropertyName("applinks")]
    [UsedImplicitly(Reason = "Objects of this class are serialized to JSON and thus the properties are required.")]
    public AppLinksModel AppLinks { get; set; }

    public class AppLinksModel
    {
        public AppLinksModel(string[]? appIds)
        {
            appIds ??= [];
            Details = [new AppLinkDetailsModel { AppIds = appIds }];
        }

        [JsonPropertyName("details")]
        [UsedImplicitly(Reason = "Objects of this class are serialized to JSON and thus the properties are required.")]
        public AppLinkDetailsModel[] Details { get; set; }

        public class AppLinkDetailsModel
        {
            [JsonPropertyName("appIDs")]
            [UsedImplicitly(Reason = "Objects of this class are serialized to JSON and thus the properties are required.")]
            public required string[] AppIds { get; set; }

            [JsonPropertyName("components")]
            [UsedImplicitly(Reason = "Objects of this class are serialized to JSON and thus the properties are required.")]
            public AppLinkComponentModel[] Components { get; set; } = [new()];

            public class AppLinkComponentModel
            {
                [JsonPropertyName("/")]
                [UsedImplicitly(Reason = "Objects of this class are serialized to JSON and thus the properties are required.")]
                public string Path { get; set; } = "/r/*";

                [JsonPropertyName("comment")]
                [UsedImplicitly(Reason = "Objects of this class are serialized to JSON and thus the properties are required.")]
                public string Comment { get; set; } = "Matches any URL whose path starts with /r/";
            }
        }
    }
}
