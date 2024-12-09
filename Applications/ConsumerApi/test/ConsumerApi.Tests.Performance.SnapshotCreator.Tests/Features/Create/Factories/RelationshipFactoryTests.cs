using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;
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

public class RelationshipFactoryTests : SnapshotCreatorTestsBase
{
    private readonly IConsumerApiHelper _consumerApiHelper;
    private readonly RelationshipFactory _sut;
    private readonly Client? _sdkCLient;

    public RelationshipFactoryTests()
    {
        var logger = A.Fake<ILogger<RelationshipFactory>>();
        _consumerApiHelper = A.Fake<IConsumerApiHelper>();
        _sdkCLient = GetSdkClient();
        _sut = new RelationshipFactory(logger, _consumerApiHelper);
    }

    [Fact]
    public async Task Create_ShouldCreateRelationships()
    {
        // Arrange
        var appIdentity = new DomainIdentity(
            null!,
            null,
            1,
            0,
            2,
            IdentityPoolType.App,
            5,
            "a1",
            2,
            0);

        var connectorIdentity = new DomainIdentity(
            null!,
            null,
            1,
            0,
            3,
            IdentityPoolType.Connector,
            5,
            "c1",
            3,
            0)
        {
            RelationshipTemplates =
            {
                new RelationshipTemplateBag(new CreateRelationshipTemplateResponse
                {
                    Id = "templateId",
                    CreatedAt = default
                }, used: false)
            }
        };

        var command = new CreateRelationships.Command(
            [appIdentity, connectorIdentity],
            [
                new RelationshipAndMessages(
                    SenderPoolAlias: appIdentity.PoolAlias,
                    SenderIdentityAddress: appIdentity.ConfigurationIdentityAddress,
                    RecipientIdentityAddress: connectorIdentity.ConfigurationIdentityAddress,
                    RecipientPoolAlias: connectorIdentity.PoolAlias)
            ],
            "http://baseurl",
            new ClientCredentials("clientId", "clientSecret")
        );

        A.CallTo(() => _consumerApiHelper.CreateForExistingIdentity(A<string>._, A<ClientCredentials>._, A<UserCredentials>._, null))!
            .Returns(_sdkCLient);

        const string expectedRelationshipId = "relationshipId-1";
        A.CallTo(() => _consumerApiHelper.CreateRelationship(A<Client>._, A<RelationshipTemplateBag>._))
            .Returns(new ApiResponse<RelationshipMetadata>()
            {
                Result = new RelationshipMetadata
                {
                    Id = expectedRelationshipId,
                    From = "null",
                    To = "null",
                    CreatedAt = default,
                    Status = "null",
                    AuditLog = null!
                }
            });

        A.CallTo(() => _consumerApiHelper.AcceptRelationship(A<Client>._, A<ApiResponse<RelationshipMetadata>>._))
            .Returns(new ApiResponse<RelationshipMetadata>()
            {
                Result = new RelationshipMetadata
                {
                    Id = expectedRelationshipId,
                    From = "null",
                    To = "null",
                    CreatedAt = DateTime.Now,
                    Status = "null",
                    AuditLog = null!
                }
            });

        // Act
        await _sut.Create(command, appIdentity, [connectorIdentity]);

        // Assert
        A.CallTo(() => _consumerApiHelper.CreateRelationship(_sdkCLient!, A<RelationshipTemplateBag>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _consumerApiHelper.AcceptRelationship(_sdkCLient!, A<ApiResponse<RelationshipMetadata>>._)).MustHaveHappenedOnceExactly();


        _sut.NumberOfCreatedRelationships.Should().Be(1);
        appIdentity.EstablishedRelationshipsById.Should().ContainKey(expectedRelationshipId);
        appIdentity.EstablishedRelationshipsById.Count.Should().Be(1);
        _sut.SemaphoreSlim.CurrentCount.Should().Be(Environment.ProcessorCount);
    }

    [Fact]
    public async Task Create_AcceptRelationshipResponseResultNull_ShouldThrowException()
    {
        // Arrange
        var appIdentity = new DomainIdentity(
            null!,
            null,
            1,
            0,
            2,
            IdentityPoolType.App,
            5,
            "a1",
            2,
            0);

        var connectorIdentity = new DomainIdentity(
            null!,
            null,
            1,
            0,
            3,
            IdentityPoolType.Connector,
            5,
            "c1",
            3,
            0)
        {
            RelationshipTemplates =
            {
                new RelationshipTemplateBag(new CreateRelationshipTemplateResponse
                {
                    Id = "templateId",
                    CreatedAt = default
                }, used: false)
            }
        };

        var command = new CreateRelationships.Command(
            [appIdentity, connectorIdentity],
            [
                new RelationshipAndMessages(
                    SenderPoolAlias: appIdentity.PoolAlias,
                    SenderIdentityAddress: appIdentity.ConfigurationIdentityAddress,
                    RecipientIdentityAddress: connectorIdentity.ConfigurationIdentityAddress,
                    RecipientPoolAlias: connectorIdentity.PoolAlias)
            ],
            "http://baseurl",
            new ClientCredentials("clientId", "clientSecret")
        );

        A.CallTo(() => _consumerApiHelper.CreateForExistingIdentity(A<string>._, A<ClientCredentials>._, A<UserCredentials>._, null))!
            .Returns(_sdkCLient);

        const string expectedRelationshipId = "relationshipId-1";
        A.CallTo(() => _consumerApiHelper.CreateRelationship(A<Client>._, A<RelationshipTemplateBag>._))
            .Returns(new ApiResponse<RelationshipMetadata>()
            {
                Result = new RelationshipMetadata
                {
                    Id = expectedRelationshipId,
                    From = "null",
                    To = "null",
                    CreatedAt = default,
                    Status = "null",
                    AuditLog = null!
                }
            });

        A.CallTo(() => _consumerApiHelper.AcceptRelationship(A<Client>._, A<ApiResponse<RelationshipMetadata>>._))
            .Returns(new ApiResponse<RelationshipMetadata>()
            {
                Result = null
            });

        // Act
        var act = () => _sut.Create(command, appIdentity, [connectorIdentity]);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();

        A.CallTo(() => _consumerApiHelper.CreateRelationship(_sdkCLient!, A<RelationshipTemplateBag>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _consumerApiHelper.AcceptRelationship(_sdkCLient!, A<ApiResponse<RelationshipMetadata>>._)).MustHaveHappenedOnceExactly();
        _sut.NumberOfCreatedRelationships.Should().Be(0);
        _sut.SemaphoreSlim.CurrentCount.Should().Be(Environment.ProcessorCount);
    }

    [Fact]
    public async Task Create_NextRelationshipTemplateIsNull_ShouldThrowException()
    {
        // Arrange
        var appIdentity = new DomainIdentity(
            null!,
            null,
            1,
            0,
            2,
            IdentityPoolType.App,
            5,
            "a1",
            2,
            0);

        var connectorIdentity = new DomainIdentity(
            null!,
            null,
            1,
            0,
            3,
            IdentityPoolType.Connector,
            5,
            "c1",
            3,
            0);

        var command = new CreateRelationships.Command(
            [appIdentity, connectorIdentity],
            [
                new RelationshipAndMessages(
                    SenderPoolAlias: appIdentity.PoolAlias,
                    SenderIdentityAddress: appIdentity.ConfigurationIdentityAddress,
                    RecipientIdentityAddress: connectorIdentity.ConfigurationIdentityAddress,
                    RecipientPoolAlias: connectorIdentity.PoolAlias)
            ],
            "http://baseurl",
            new ClientCredentials("clientId", "clientSecret")
        );

        A.CallTo(() => _consumerApiHelper.CreateForExistingIdentity(A<string>._, A<ClientCredentials>._, A<UserCredentials>._, null))!
            .Returns(_sdkCLient);

        const string expectedRelationshipId = "relationshipId-1";
        A.CallTo(() => _consumerApiHelper.CreateRelationship(A<Client>._, A<RelationshipTemplateBag>._))
            .Returns(new ApiResponse<RelationshipMetadata>()
            {
                Result = new RelationshipMetadata
                {
                    Id = expectedRelationshipId,
                    From = "null",
                    To = "null",
                    CreatedAt = default,
                    Status = "null",
                    AuditLog = null!
                }
            });

        A.CallTo(() => _consumerApiHelper.AcceptRelationship(A<Client>._, A<ApiResponse<RelationshipMetadata>>._))
            .Returns(new ApiResponse<RelationshipMetadata>()
            {
                Result = new RelationshipMetadata
                {
                    Id = expectedRelationshipId,
                    From = "null",
                    To = "null",
                    CreatedAt = DateTime.Now,
                    Status = "null",
                    AuditLog = null!
                }
            });

        // Act
        var act = () => _sut.Create(command, appIdentity, [connectorIdentity]);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();

        A.CallTo(() => _consumerApiHelper.CreateRelationship(_sdkCLient!, A<RelationshipTemplateBag>._)).MustNotHaveHappened();
        A.CallTo(() => _consumerApiHelper.AcceptRelationship(_sdkCLient!, A<ApiResponse<RelationshipMetadata>>._)).MustNotHaveHappened();
        _sut.NumberOfCreatedRelationships.Should().Be(0);
        _sut.SemaphoreSlim.CurrentCount.Should().Be(Environment.ProcessorCount);
    }

    [Fact]
    public async Task Create_CreateRelationshipResponseIsError_ShouldThrowException()
    {
        // Arrange
        var appIdentity = new DomainIdentity(
            null!,
            null,
            1,
            0,
            2,
            IdentityPoolType.App,
            5,
            "a1",
            2,
            0);

        var connectorIdentity = new DomainIdentity(
            null!,
            null,
            1,
            0,
            3,
            IdentityPoolType.Connector,
            5,
            "c1",
            3,
            0)
        {
            RelationshipTemplates =
            {
                new RelationshipTemplateBag(new CreateRelationshipTemplateResponse
                {
                    Id = "templateId",
                    CreatedAt = default
                }, used: false)
            }
        };

        var command = new CreateRelationships.Command(
            [appIdentity, connectorIdentity],
            [
                new RelationshipAndMessages(
                    SenderPoolAlias: appIdentity.PoolAlias,
                    SenderIdentityAddress: appIdentity.ConfigurationIdentityAddress,
                    RecipientIdentityAddress: connectorIdentity.ConfigurationIdentityAddress,
                    RecipientPoolAlias: connectorIdentity.PoolAlias)
            ],
            "http://baseurl",
            new ClientCredentials("clientId", "clientSecret")
        );

        A.CallTo(() => _consumerApiHelper.CreateForExistingIdentity(A<string>._, A<ClientCredentials>._, A<UserCredentials>._, null))!
            .Returns(_sdkCLient);

        const string expectedRelationshipId = "relationshipId-1";
        A.CallTo(() => _consumerApiHelper.CreateRelationship(A<Client>._, A<RelationshipTemplateBag>._))
            .Returns(new ApiResponse<RelationshipMetadata>()
            {
                Result = new RelationshipMetadata
                {
                    Id = expectedRelationshipId,
                    From = "null",
                    To = "null",
                    CreatedAt = default,
                    Status = "null",
                    AuditLog = null!
                },
                Error = new ApiError
                {
                    Code = "code",
                    Message = "message",
                    Id = "null",
                    Docs = "null",
                    Time = DateTime.Now
                }
            });

        A.CallTo(() => _consumerApiHelper.AcceptRelationship(A<Client>._, A<ApiResponse<RelationshipMetadata>>._))
            .Returns(new ApiResponse<RelationshipMetadata>()
            {
                Result = new RelationshipMetadata
                {
                    Id = expectedRelationshipId,
                    From = "null",
                    To = "null",
                    CreatedAt = DateTime.Now,
                    Status = "null",
                    AuditLog = null!
                }
            });

        // Act
        var act = () => _sut.Create(command, appIdentity, [connectorIdentity]);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();

        A.CallTo(() => _consumerApiHelper.CreateRelationship(_sdkCLient!, A<RelationshipTemplateBag>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _consumerApiHelper.AcceptRelationship(_sdkCLient!, A<ApiResponse<RelationshipMetadata>>._)).MustNotHaveHappened();
        _sut.NumberOfCreatedRelationships.Should().Be(0);
        _sut.SemaphoreSlim.CurrentCount.Should().Be(Environment.ProcessorCount);
    }

    [Fact]
    public async Task Create_CreateRelationshipResponseIsNull_ShouldThrowException()
    {
        // Arrange
        var appIdentity = new DomainIdentity(
            null!,
            null,
            1,
            0,
            2,
            IdentityPoolType.App,
            5,
            "a1",
            2,
            0);

        var connectorIdentity = new DomainIdentity(
            null!,
            null,
            1,
            0,
            3,
            IdentityPoolType.Connector,
            5,
            "c1",
            3,
            0)
        {
            RelationshipTemplates =
            {
                new RelationshipTemplateBag(new CreateRelationshipTemplateResponse
                {
                    Id = "templateId",
                    CreatedAt = default
                }, used: false)
            }
        };

        var command = new CreateRelationships.Command(
            [appIdentity, connectorIdentity],
            [
                new RelationshipAndMessages(
                    SenderPoolAlias: appIdentity.PoolAlias,
                    SenderIdentityAddress: appIdentity.ConfigurationIdentityAddress,
                    RecipientIdentityAddress: connectorIdentity.ConfigurationIdentityAddress,
                    RecipientPoolAlias: connectorIdentity.PoolAlias)
            ],
            "http://baseurl",
            new ClientCredentials("clientId", "clientSecret")
        );

        A.CallTo(() => _consumerApiHelper.CreateForExistingIdentity(A<string>._, A<ClientCredentials>._, A<UserCredentials>._, null))!
            .Returns(_sdkCLient);

        const string expectedRelationshipId = "relationshipId-1";
        A.CallTo(() => _consumerApiHelper.CreateRelationship(A<Client>._, A<RelationshipTemplateBag>._))
            .Returns(new ApiResponse<RelationshipMetadata>()
            {
                Result = null
            });

        A.CallTo(() => _consumerApiHelper.AcceptRelationship(A<Client>._, A<ApiResponse<RelationshipMetadata>>._))
            .Returns(new ApiResponse<RelationshipMetadata>()
            {
                Result = new RelationshipMetadata
                {
                    Id = expectedRelationshipId,
                    From = "null",
                    To = "null",
                    CreatedAt = DateTime.Now,
                    Status = "null",
                    AuditLog = null!
                }
            });

        // Act
        var act = () => _sut.Create(command, appIdentity, [connectorIdentity]);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();

        A.CallTo(() => _consumerApiHelper.CreateRelationship(_sdkCLient!, A<RelationshipTemplateBag>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _consumerApiHelper.AcceptRelationship(_sdkCLient!, A<ApiResponse<RelationshipMetadata>>._)).MustNotHaveHappened();
        _sut.NumberOfCreatedRelationships.Should().Be(0);
        _sut.SemaphoreSlim.CurrentCount.Should().Be(Environment.ProcessorCount);
    }

    [Fact]
    public async Task Create_AcceptRelationshipResponseIsError_ShouldThrowException()
    {
        // Arrange
        var appIdentity = new DomainIdentity(
            null!,
            null,
            1,
            0,
            2,
            IdentityPoolType.App,
            5,
            "a1",
            2,
            0);

        var connectorIdentity = new DomainIdentity(
            null!,
            null,
            1,
            0,
            3,
            IdentityPoolType.Connector,
            5,
            "c1",
            3,
            0)
        {
            RelationshipTemplates =
            {
                new RelationshipTemplateBag(new CreateRelationshipTemplateResponse
                {
                    Id = "templateId",
                    CreatedAt = default
                }, used: false)
            }
        };

        var command = new CreateRelationships.Command(
            [appIdentity, connectorIdentity],
            [
                new RelationshipAndMessages(
                    SenderPoolAlias: appIdentity.PoolAlias,
                    SenderIdentityAddress: appIdentity.ConfigurationIdentityAddress,
                    RecipientIdentityAddress: connectorIdentity.ConfigurationIdentityAddress,
                    RecipientPoolAlias: connectorIdentity.PoolAlias)
            ],
            "http://baseurl",
            new ClientCredentials("clientId", "clientSecret")
        );

        A.CallTo(() => _consumerApiHelper.CreateForExistingIdentity(A<string>._, A<ClientCredentials>._, A<UserCredentials>._, null))!
            .Returns(_sdkCLient);

        const string expectedRelationshipId = "relationshipId-1";
        A.CallTo(() => _consumerApiHelper.CreateRelationship(A<Client>._, A<RelationshipTemplateBag>._))
            .Returns(new ApiResponse<RelationshipMetadata>()
            {
                Result = new RelationshipMetadata
                {
                    Id = expectedRelationshipId,
                    From = "null",
                    To = "null",
                    CreatedAt = DateTime.Now,
                    Status = "null",
                    AuditLog = null!
                }
            });

        A.CallTo(() => _consumerApiHelper.AcceptRelationship(A<Client>._, A<ApiResponse<RelationshipMetadata>>._))
            .Returns(new ApiResponse<RelationshipMetadata>()
            {
                Result = new RelationshipMetadata
                {
                    Id = expectedRelationshipId,
                    From = "null",
                    To = "null",
                    CreatedAt = default,
                    Status = "null",
                    AuditLog = null!
                },
                Error = new ApiError
                {
                    Code = "code",
                    Message = "message",
                    Id = "null",
                    Docs = "null",
                    Time = DateTime.Now
                }
            });

        // Act
        var act = () => _sut.Create(command, appIdentity, [connectorIdentity]);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();

        A.CallTo(() => _consumerApiHelper.CreateRelationship(_sdkCLient!, A<RelationshipTemplateBag>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _consumerApiHelper.AcceptRelationship(_sdkCLient!, A<ApiResponse<RelationshipMetadata>>._)).MustHaveHappenedOnceExactly();
        _sut.NumberOfCreatedRelationships.Should().Be(0);
        _sut.SemaphoreSlim.CurrentCount.Should().Be(Environment.ProcessorCount);
    }
}
