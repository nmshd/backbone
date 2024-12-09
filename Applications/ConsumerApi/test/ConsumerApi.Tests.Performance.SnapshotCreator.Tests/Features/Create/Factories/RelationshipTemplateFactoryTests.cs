using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Responses;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Helper;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Enums;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Create.Factories;

public class RelationshipTemplateFactoryTests : SnapshotCreatorTestsBase
{
    private readonly RelationshipTemplateFactory _sut;
    private readonly IConsumerApiHelper _consumerApiHelper;
    private readonly Client? _sdkClient;

    public RelationshipTemplateFactoryTests()
    {
        _sdkClient = GetSdkClient();
        var logger = A.Fake<ILogger<RelationshipTemplateFactory>>();
        _consumerApiHelper = A.Fake<IConsumerApiHelper>();
        _sut = new RelationshipTemplateFactory(logger, _consumerApiHelper);
    }

    [Fact]
    public async Task Create_ShouldCreateRelationshipTemplates()
    {
        // Arrange
        var command = new CreateRelationshipTemplates.Command(
            [
                new DomainIdentity(null!, null, 0, 0, 1, IdentityPoolType.App, 5, "", 2, 0)
            ],
            "http://baseurl",
            new ClientCredentials("clientId", "clientSecret")
        );

        var identity = command.Identities[0];
        var expectedTotalRelationshipTemplates = identity.NumberOfRelationshipTemplates;


        A.CallTo(() => _consumerApiHelper.CreateForExistingIdentity(command.BaseUrlAddress, command.ClientCredentials, identity.UserCredentials, null))!
            .Returns(_sdkClient);

        var relationshipTemplateResponse = new ApiResponse<CreateRelationshipTemplateResponse>
        {
            Result = new CreateRelationshipTemplateResponse
            {
                Id = "null",
                CreatedAt = default
            }
        };

        A.CallTo(() => _consumerApiHelper.CreateRelationshipTemplate(_sdkClient!))
            .Returns(relationshipTemplateResponse);

        // Act
        await _sut.Create(command, identity);

        // Assert
        identity.RelationshipTemplates.Should().HaveCount(expectedTotalRelationshipTemplates);
        A.CallTo(() => _consumerApiHelper.CreateRelationshipTemplate(_sdkClient!)).MustHaveHappened(expectedTotalRelationshipTemplates, Times.Exactly);
        _sut.TotalCreatedRelationshipTemplates.Should().Be(expectedTotalRelationshipTemplates);
        _sut.GetSemaphoreCurrentCount().Should().Be(Environment.ProcessorCount);
    }


    [Fact]
    public async Task Create_RelationshipTemplateResponseNull_ShouldNotCreateRelationshipTemplates()
    {
        // Arrange
        var command = new CreateRelationshipTemplates.Command(
            [
                new DomainIdentity(null!, null, 0, 0, 1, IdentityPoolType.App, 5, "", 2, 0)
            ],
            "http://baseurl",
            new ClientCredentials("clientId", "clientSecret")
        );

        var identity = command.Identities[0];

        A.CallTo(() => _consumerApiHelper.CreateForExistingIdentity(command.BaseUrlAddress, command.ClientCredentials, identity.UserCredentials, null))!
            .Returns(_sdkClient);

        var relationshipTemplateResponse = new ApiResponse<CreateRelationshipTemplateResponse>
        {
            Result = null
        };

        const int expectedTotalRelationshipTemplates = 0;
        var expectedNumCreateRelationshipTemplateCalls = identity.NumberOfRelationshipTemplates;

        A.CallTo(() => _consumerApiHelper.CreateRelationshipTemplate(_sdkClient!))
            .Returns(relationshipTemplateResponse);

        // Act
        await _sut.Create(command, identity);

        // Assert
        identity.RelationshipTemplates.Should().HaveCount(expectedTotalRelationshipTemplates);
        A.CallTo(() => _consumerApiHelper.CreateRelationshipTemplate(_sdkClient!)).MustHaveHappened(expectedNumCreateRelationshipTemplateCalls, Times.Exactly);
        _sut.TotalCreatedRelationshipTemplates.Should().Be(expectedTotalRelationshipTemplates);
        _sut.GetSemaphoreCurrentCount().Should().Be(Environment.ProcessorCount);
    }


    [Fact]
    public async Task Create_RelationshipTemplateResponseIsError_ShouldThrowException()
    {
        // Arrange
        var command = new CreateRelationshipTemplates.Command(
            [
                new DomainIdentity(null!, null, 0, 0, 1, IdentityPoolType.App, 5, "", 2, 0)
            ],
            "http://baseurl",
            new ClientCredentials("clientId", "clientSecret")
        );

        var identity = command.Identities[0];


        A.CallTo(() => _consumerApiHelper.CreateForExistingIdentity(command.BaseUrlAddress, command.ClientCredentials, identity.UserCredentials, null))!
            .Returns(_sdkClient);

        var relationshipTemplateResponse = new ApiResponse<CreateRelationshipTemplateResponse>
        {
            Error = new ApiError
            {
                Id = "null",
                Code = "null",
                Message = "null",
                Docs = "null",
                Time = default
            },
            Result = new CreateRelationshipTemplateResponse
            {
                Id = "null",
                CreatedAt = default
            }
        };

        A.CallTo(() => _consumerApiHelper.CreateRelationshipTemplate(_sdkClient!))
            .Returns(relationshipTemplateResponse);

        // Act
        var act = () => _sut.Create(command, identity);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        _sut.GetSemaphoreCurrentCount().Should().Be(Environment.ProcessorCount);
    }
}
