using System.Text.Json.Serialization;
using Backbone.ConsumerApi.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi.Controllers.Onboarding;

[Route(".well-known")]
public class WellKnownController : Controller
{
    private readonly ConsumerApiConfiguration.WellKnownPreviewConfiguration _configuration;

    public WellKnownController(IOptions<ConsumerApiConfiguration> configuration)
    {
        _configuration = configuration.Value.WellKnownPreview;
    }

    [HttpGet("apple-app-site-association")]
    public IActionResult AppleAppSiteAssociation()
    {
        return Json(new AppleAppSiteAssociation(_configuration.AppleApps));
    }

    [HttpGet("assetlinks.json")]
    public IActionResult Apple()
    {
        return Json(_configuration.AndroidApps.Select(a => new AndroidAssetLink(
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
    public string[] Relation { get; set; } = ["delegate_permission/common.handle_all_urls"];

    [JsonPropertyName("target")]
    public TargetModel Target { get; set; }

    public class TargetModel
    {
        [JsonPropertyName("namespace")]
        public string Namespace { get; set; } = "android_app";

        [JsonPropertyName("package_name")]
        public required string PackageName { get; set; }

        [JsonPropertyName("sha256_cert_fingerprints")]
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
    public AppLinksModel AppLinks { get; set; }

    public class AppLinksModel
    {
        public AppLinksModel(string[]? appIds)
        {
            appIds ??= [];
            Details = [new AppLinkDetailsModel { AppIds = appIds }];
        }

        [JsonPropertyName("details")]
        public AppLinkDetailsModel[] Details { get; set; }

        public class AppLinkDetailsModel
        {
            [JsonPropertyName("appIDs")]
            public required string[] AppIds { get; set; }

            [JsonPropertyName("components")]
            public AppLinkComponentModel[] Components { get; set; } = [new()];

            public class AppLinkComponentModel
            {
                [JsonPropertyName("/")]
                public string Path { get; set; } = "/r/*";

                [JsonPropertyName("comment")]
                public string Comment { get; set; } = "Matches any URL whose path starts with /r/";
            }
        }
    }
}
