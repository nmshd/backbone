using Backbone.AdminApi.Sdk.Endpoints.Announcements.Types;
using Backbone.AdminApi.Sdk.Endpoints.Announcements.Types.Requests;
using Backbone.AdminApi.Sdk.Endpoints.Announcements.Types.Responses;
using Backbone.AdminApi.Tests.Integration.Configuration;
using Backbone.AdminApi.Tests.Integration.Extensions;
using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;
using Microsoft.Extensions.Options;

namespace Backbone.AdminApi.Tests.Integration.StepDefinitions;

[Binding]
[Scope(Feature = "GET /Announcements")]
[Scope(Feature = "DELETE /Announcements/{id}")]
[Scope(Feature = "POST /Announcements")]
internal class AnnouncementsStepDefinitions : BaseStepDefinitions
{
    private ApiResponse<Announcement>? _createAnnouncementResponse;
    private ApiResponse<ListAnnouncementsResponse>? _announcementsResponse;
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
            Severity = AnnouncementSeverity.Medium,
            IsSilent = false,
            Texts =
            [
                new CreateAnnouncementRequestText
                {
                    Language = "en",
                    Title = "Title",
                    Body = "Body"
                }
            ],
            ExpiresAt = DateTime.UtcNow,
            Actions = []
        });

        _givenAnnouncement = response.Result;
    }

    [When("^a GET request is sent to the /Announcements endpoint$")]
    public async Task WhenAGETRequestIsSentToTheAnnouncementsEndpoint()
    {
        _whenResponse = _announcementsResponse = await _client.Announcements.ListAnnouncements();
    }

    [When("^a POST request is sent to the /Announcements endpoint with a valid content$")]
    public async Task WhenAPOSTRequestIsSentToTheAnnouncementsEndpointWithAValidContent()
    {
        _whenResponse = _createAnnouncementResponse = await _client.Announcements.CreateAnnouncement(new CreateAnnouncementRequest
        {
            Severity = AnnouncementSeverity.High,
            IsSilent = false,
            ExpiresAt = SystemTime.UtcNow.AddDays(1),
            Texts =
            [
                new CreateAnnouncementRequestText
                {
                    Language = "en",
                    Title = "Title",
                    Body = "Body"
                }
            ],
            Actions = []
        });
    }

    [When("^a POST request is sent to the /Announcements endpoint without an English translation$")]
    public async Task WhenAPOSTRequestIsSentToTheAnnouncementsEndpointWithoutAnEnglishTranslation()
    {
        _whenResponse = _createAnnouncementResponse = await _client.Announcements.CreateAnnouncement(new CreateAnnouncementRequest
        {
            Severity = AnnouncementSeverity.High,
            IsSilent = false,
            ExpiresAt = SystemTime.UtcNow.AddDays(1),
            Texts =
            [
                new CreateAnnouncementRequestText
                {
                    Language = "de",
                    Title = "Titel",
                    Body = "Inhalt"
                }
            ],
            Actions = []
        });
    }

    [When("^a POST request is sent to the /Announcements endpoint with an action$")]
    public async Task WhenAPOSTRequestIsSentToTheAnnouncementsEndpointWithAnAction()
    {
        _whenResponse = _createAnnouncementResponse = await _client.Announcements.CreateAnnouncement(new CreateAnnouncementRequest
        {
            Severity = AnnouncementSeverity.High,
            IsSilent = true,
            ExpiresAt = SystemTime.UtcNow.AddDays(1),
            Texts =
            [
                new CreateAnnouncementRequestText
                {
                    Language = "en",
                    Title = "Please provide feedback",
                    Body = "We would like to hear your thoughts on our service."
                }
            ],
            Actions =
            [
                new CreateAnnouncementRequestAction
                {
                    DisplayName = new Dictionary<string, string>
                    {
                        { "en", "Give feedback" }
                    },
                    Link = "https://enmeshed.eu/feedback"
                }
            ]
        });
    }

    [When("^a POST request is sent to the /Announcements endpoint with isSilent=false and a non-empty IQL query$")]
    public async Task WhenAPOSTRequestIsSentToTheAnnouncementsEndpointWithIsSilentFalseAndANonEmptyIqlQuery()
    {
        _whenResponse = _createAnnouncementResponse = await _client.Announcements.CreateAnnouncement(new CreateAnnouncementRequest
        {
            Severity = AnnouncementSeverity.High,
            IsSilent = false,
            ExpiresAt = SystemTime.UtcNow.AddDays(1),
            Texts =
            [
                new CreateAnnouncementRequestText
                {
                    Language = "en",
                    Title = "Title",
                    Body = "Body"
                }
            ],
            Actions = [],
            IqlQuery = "StreetAddress.city='Heidelberg' && #'Primary Address'"
        });
    }

    [When("^a DELETE request is sent to the /Announcements/{id} endpoint with a.Id$")]
    public async Task WhenADeleteRequestIsSentToTheAnnouncementsAIdEndpoint()
    {
        _whenResponse = await _client.Announcements.DeleteById(_givenAnnouncement!.Id);
    }

    [When("^a DELETE request is sent to the /Announcements/{id} endpoint with a non existing id$")]
    public async Task WhenADeleteRequestIsSentToTheAnnouncementsIdEndpointWithANonExistingId()
    {
        _whenResponse = await _client.Announcements.DeleteById("ANCnonExistingId0000");
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
        _createAnnouncementResponse!.Result.ShouldNotBeNull();
        await _createAnnouncementResponse.ShouldComplyWithSchema();
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

    [Then("the response contains the action")]
    public void ThenTheResponseContainsTheAction()
    {
        _createAnnouncementResponse!.Result.ShouldNotBeNull();
        _createAnnouncementResponse.Result!.Actions.ShouldHaveCount(1);
        _createAnnouncementResponse.Result.Actions.First().DisplayName.ShouldContainKey("en");
        _createAnnouncementResponse.Result.Actions.First().DisplayName["en"].ShouldBe("Give feedback");
        _createAnnouncementResponse.Result.Actions.First().Link.ShouldBe("https://enmeshed.eu/feedback");
    }

    [Then("the Announcement a does not exist anymore")]
    public async Task ThenTheAnnouncementADoesNotExistAnymore()
    {
        var announcements = await _client.Announcements.ListAnnouncements();
        announcements.Result!.ShouldNotContain(a => a.Id == _givenAnnouncement!.Id);
    }
}
