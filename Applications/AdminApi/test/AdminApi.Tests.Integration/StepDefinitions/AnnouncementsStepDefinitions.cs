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

    [When(@"a GET request is sent to the /Announcements endpoint")]
    public async Task WhenAGETRequestIsSentToTheAnnouncementsEndpoint()
    {
        _announcementsResponse = await _client.Announcements.GetAllAnnouncements();
    }

    [When(@"a POST request is sent to the /Announcements endpoint with a valid content")]
    public async Task WhenAPOSTRequestIsSentToTheAnnouncementsEndpointWithAValidContent()
    {
        _announcementResponse = await _client.Announcements.CreateAnnouncement(new CreateAnnouncementRequest
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

    [Then(@"the response status code is (\d+) \(.+\)")]
    public void ThenTheResponseStatusCodeIs(int expectedStatusCode)
    {
        if (_announcementResponse != null)
            ((int)_announcementResponse.Status).Should().Be(expectedStatusCode);

        if (_announcementsResponse != null)
            ((int)_announcementsResponse.Status).Should().Be(expectedStatusCode);
    }

    [Then(@"the response contains an Announcement")]
    public async Task ThenTheResponseContainsAnAnnouncement()
    {
        _announcementResponse!.Result.Should().NotBeNull();
        await _announcementResponse.Should().ComplyWithSchema();
    }

    [Then(@"the response contains a list of Announcements")]
    public async Task ThenTheResponseContainsAListOfAnnouncements()
    {
        _announcementsResponse!.Result.Should().NotBeNull();
        await _announcementsResponse.Should().ComplyWithSchema();
    }

    [Then(@"the response contains the Announcement a")]
    public void ThenTheResponseContainsTheAnnouncementA()
    {
        _announcementsResponse!.Result.Should().ContainSingle(a => a.Id == _givenAnnouncement!.Id);
    }
}
