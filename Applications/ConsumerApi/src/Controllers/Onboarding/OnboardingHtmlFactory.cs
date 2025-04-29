using Backbone.ConsumerApi.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.ConsumerApi.Controllers.Onboarding;

public class OnboardingHtmlFactory : Controller
{
    public IActionResult GenerateOnboardingPage(ConsumerApiConfiguration.OnboardingConfiguration appConfiguration, List<Tuple<string, string>> appStoreLinksToDisplay)
    {
        const string header = "<head><title>Onboarding Page</title></head>";

        // The url will look something like this: BASE_URL/resource/RESOURCE_ID[...]#SECRET_KEY
        // The value after the hash is needed to open the resource within the app. However, this value is - by design - not available for the backbone.
        // Hence, we set the correct link only on the user's device, with the help of this js-code.
        const string jsCodeToSetInAppLink =
            "<script>document.addEventListener('DOMContentLoaded', function () {var link = document.getElementById('inAppLink');link.href = window.location;console.log(link.href);});</script>";
        var inAppLink = $"<br><a href=\" \" id=\"inAppLink\">Open {appConfiguration.AppNameIdentifier} App</a><br>";

        const string onboardingText = "<p>You have opened an object with respect to tokenXX ...  to complete this process do either of the following:</p>";
        const string appAlreadyInstalled = "<p>App already installed?</p> Then please click the link below to open the app directly:<br>";

        var appStoresComponent = "<p>App not yet installed?</p> Then please click the link below to install the app:<br>";
        foreach (var appStoreLink in appStoreLinksToDisplay)
            appStoresComponent += $"<a href={appStoreLink.Item2} target=\"_blank\"> Install via {appStoreLink.Item1}</a><br>";

        return base.Content(header + jsCodeToSetInAppLink + onboardingText + "<br>" + appAlreadyInstalled + inAppLink + appStoresComponent, "text/html");
    }

    public IActionResult GenerateAppSelectionPage(ConsumerApiConfiguration configuration, HttpRequest request)
    {
        const string header = "<!DOCTYPE html><head><title>Onboarding Page</title></head>";
        var body = "<p>Please pick the application specified by your provider and install it.</p><br>";
        var countAvailableApps = configuration.Onboarding.Length;

        var functionsToAddHashValuesToAllLinks = "<script>";
        for (var i = 0; i < countAvailableApps; i++)
        {
            var onboardingConfiguration = configuration.Onboarding[i];

            // The correct url will look something like this: BASE_URL/resource/RESOURCE_ID?appName=name#SECRET_KEY
            // The value after the hash is needed to open the resource within the app. However, this value is - by design - not available for the backbone.
            // Hence, we set the correct link only on the user's device, with the help of this js-code.
            body += $"<a href=\"{request.Path.Value!}?appName={onboardingConfiguration.AppNameIdentifier}\" id=\"inAppLink{i}\">Install app {onboardingConfiguration.AppNameIdentifier}</a><br>";
            functionsToAddHashValuesToAllLinks +=
                $"document.addEventListener('DOMContentLoaded', function () {{var link = document.getElementById('inAppLink{i}');link.href += window.location.hash;console.log(link.href);}});";
        }

        functionsToAddHashValuesToAllLinks += "</script>";

        return base.Content(header + functionsToAddHashValuesToAllLinks + body, "text/html");
    }
}
