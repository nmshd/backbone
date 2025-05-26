using Backbone.AdminApi.Sdk.Endpoints.Announcements.Types;
using Backbone.AdminApi.Sdk.Endpoints.Announcements.Types.Requests;
using Backbone.AdminApi.Sdk.Endpoints.Announcements.Types.Responses;
using Backbone.AdminApi.Tests.Integration.Configuration;
using Backbone.AdminApi.Tests.Integration.Extensions;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.Tooling;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET /Announcements")]
[Scope(Feature = "POST /Announcements")]
internal class AnnouncementsStepDefinitions : BaseStepDefinitions
{
    private ApiResponse<Announcement>? _announcementResponse;
    private ApiResponse<GetAllAnnouncementsResponse>? _announcementsResponse;
    private IResponse? _whenResponse;
    private Announcement? _givenAnnouncement;

    public AnnouncementsStepDefinitions(HttpClientFactory factory, IOptions<HttpClientOptions> options) : base(factory, options)
    {
    }

    [Given(@"an existing Announcement a")]
    public async Task GivenAnExistingAnnouncementA()
    {
        var response = await _client.Announcements.CreateAnnouncement(new CreateAnnouncementRequest
        {
            Texts =
            [
                new CreateAnnouncementRequestText
                {
                    Language = "en",
                    Title = "Title",
                    Body = "Body"
                }
            ],
            Severity = AnnouncementSeverity.Medium,
            ExpiresAt = DateTime.UtcNow
        });

        _givenAnnouncement = response.Result;
    }

    [When("^a GET request is sent to the /Announcements endpoint$")]
    public async Task WhenAGETRequestIsSentToTheAnnouncementsEndpoint()
    {
        _whenResponse = _announcementsResponse = await _client.Announcements.GetAllAnnouncements();
    }

    [When("^a POST request is sent to the /Announcements endpoint with a valid content$")]
    public async Task WhenAPOSTRequestIsSentToTheAnnouncementsEndpointWithAValidContent()
    {
        _whenResponse = _announcementResponse = await _client.Announcements.CreateAnnouncement(new CreateAnnouncementRequest
        {
            Severity = AnnouncementSeverity.High,
            ExpiresAt = SystemTime.UtcNow.AddDays(1),
            Texts =
            [
                new CreateAnnouncementRequestText
                {
                    Language = "en",
                    Title = "Title",
                    Body = "Body"
                }
            ]
        });
    }

    [When("^a POST request is sent to the /Announcements endpoint without an English translation$")]
    public async Task WhenAPOSTRequestIsSentToTheAnnouncementsEndpointWithoutAnEnglishTranslation()
    {
        _whenResponse = _announcementResponse = await _client.Announcements.CreateAnnouncement(new CreateAnnouncementRequest
        {
            Severity = AnnouncementSeverity.High,
            ExpiresAt = SystemTime.UtcNow.AddDays(1),
            Texts =
            [
                new CreateAnnouncementRequestText
                {
                    Language = "de",
                    Title = "Titel",
                    Body = "Inhalt"
                }
            ]
        });
    }

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        _whenResponse.ShouldNotBeNull();
        ((int)_whenResponse!.Status).ShouldBe(expectedStatusCode);
    }

    [Then(@"the response contains an Announcement")]
    public async Task ThenTheResponseContainsAnAnnouncement()
    {
        _announcementResponse!.Result.ShouldNotBeNull();
        await _announcementResponse.ShouldComplyWithSchema();
    }

    [Then(@"the response contains a list of Announcements")]
    public async Task ThenTheResponseContainsAListOfAnnouncements()
    {
        _announcementsResponse!.Result.ShouldNotBeNull();
        await _announcementsResponse.ShouldComplyWithSchema();
    }

    [Then(@"the response contains the Announcement a")]
    public void ThenTheResponseContainsTheAnnouncementA()
    {
        _announcementsResponse!.Result!.ShouldContain(a => a.Id == _givenAnnouncement!.Id);
    }

    [Then(@"the response content contains an error with the error code ""([^""]+)""")]
    public void ThenTheResponseContentIncludesAnErrorWithTheErrorCode(string errorCode)
    {
        _whenResponse.ShouldNotBeNull();
        _whenResponse!.Error.ShouldNotBeNull();
        _whenResponse.Error!.Code.ShouldBe(errorCode);
    }
}
