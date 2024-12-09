using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Responses;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Helper;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Enums;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Backbone.Crypto;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Create.Factories;

public class MessageFactoryTests : SnapshotCreatorTestsBase
{
    private readonly IConsumerApiHelper _consumerApiHelper;
    private readonly MessageFactory _sut;
    private readonly Client? _sdkCLient;

    public MessageFactoryTests()
    {
        var logger = A.Fake<ILogger<MessageFactory>>();
        _consumerApiHelper = A.Fake<IConsumerApiHelper>();
        _sut = new MessageFactory(logger, _consumerApiHelper);
        _sdkCLient = GetSdkClient();
    }

    [Fact]
    public async Task Create_ShouldCreateMessages()
    {
        // Arrange
        var senderIdentity = new DomainIdentity(
            null!,
            null,
            1,
            0,
            2,
            IdentityPoolType.App,
            5,
            "a1",
            2,
            10);
        var recipientIdentity = new DomainIdentity(
            null!,
            new IdentityData
            {
                Address = "identityAddress",
                KeyPair = new KeyPair(publicKey: ConvertibleString.FromUtf8("public-key"), privateKey: ConvertibleString.FromUtf8("private-key"))
            },
            1,
            0,
            3,
            IdentityPoolType.Connector,
            5,
            "c1",
            3,
            20);

        var identities = new List<DomainIdentity>
        {
            senderIdentity,
            recipientIdentity
        };

        var relationshipAndMessages = new List<RelationshipAndMessages>
        {
            new(senderIdentity.PoolAlias, senderIdentity.ConfigurationIdentityAddress, recipientIdentity.PoolAlias, recipientIdentity.ConfigurationIdentityAddress)
            {
                NumberOfSentMessages = senderIdentity.NumberOfSentMessages
            }
        };

        var command = new CreateMessages.Command(
            identities,
            relationshipAndMessages,
            "http://baseurl",
            new ClientCredentials("clientId", "clientSecret"));


        A.CallTo(() => _consumerApiHelper.CreateForExistingIdentity(command.BaseUrlAddress, command.ClientCredentials, senderIdentity.UserCredentials, null))!
            .Returns(_sdkCLient);


        A.CallTo(() => _consumerApiHelper.SendMessage(recipientIdentity, _sdkCLient!))
            .Returns(new ApiResponse<SendMessageResponse>
            {
                Result = new SendMessageResponse
                {
                    Id = "message-",
                    CreatedAt = DateTime.Now
                }
            });
        // Act
        await _sut.Create(command, senderIdentity);

        // Assert
        A.CallTo(() => _consumerApiHelper.SendMessage(recipientIdentity, _sdkCLient!))
            .MustHaveHappened(senderIdentity.NumberOfSentMessages, Times.Exactly);

        _sut.TotalCreatedMessages.Should().Be(senderIdentity.NumberOfSentMessages);
        senderIdentity.SentMessages.Should().HaveCount(senderIdentity.NumberOfSentMessages);
        _sut.GetCreateSemaphoreCurrentCount().Should().Be(Environment.ProcessorCount);
        _sut.GetCreateMessagesSemaphoreCurrentCount().Should().Be(Environment.ProcessorCount);
    }


    [Fact]
    public async Task Create_RelationshipConfiguratonMismatch_ShouldThrowException()
    {
        // Arrange
        var senderIdentity = new DomainIdentity(
            null!,
            null,
            1,
            0,
            2,
            IdentityPoolType.App,
            5,
            "a1",
            2,
            10);
        var recipientIdentity = new DomainIdentity(
            null!,
            new IdentityData
            {
                Address = "identityAddress",
                KeyPair = new KeyPair(publicKey: ConvertibleString.FromUtf8("public-key"), privateKey: ConvertibleString.FromUtf8("private-key"))
            },
            1,
            0,
            3,
            IdentityPoolType.Connector,
            5,
            "c1",
            3,
            20);

        var identities = new List<DomainIdentity>
        {
            senderIdentity,
            recipientIdentity
        };

        var relationshipAndMessages = new List<RelationshipAndMessages>
        {
            new(senderIdentity.PoolAlias, senderIdentity.ConfigurationIdentityAddress, "c3", recipientIdentity.ConfigurationIdentityAddress)
            {
                NumberOfSentMessages = senderIdentity.NumberOfSentMessages
            }
        };

        var command = new CreateMessages.Command(
            identities,
            relationshipAndMessages,
            "http://baseurl",
            new ClientCredentials("clientId", "clientSecret"));


        A.CallTo(() => _consumerApiHelper.CreateForExistingIdentity(command.BaseUrlAddress, command.ClientCredentials, senderIdentity.UserCredentials, null))!
            .Returns(_sdkCLient);


        A.CallTo(() => _consumerApiHelper.SendMessage(recipientIdentity, _sdkCLient!))
            .Returns(new ApiResponse<SendMessageResponse>
            {
                Result = new SendMessageResponse
                {
                    Id = "message-",
                    CreatedAt = DateTime.Now
                }
            });
        // Act
        var act = () => _sut.Create(command, senderIdentity);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();

        A.CallTo(() => _consumerApiHelper.SendMessage(recipientIdentity, _sdkCLient!))
            .MustNotHaveHappened();

        _sut.GetCreateSemaphoreCurrentCount().Should().Be(Environment.ProcessorCount);
        _sut.GetCreateMessagesSemaphoreCurrentCount().Should().Be(Environment.ProcessorCount);
    }


    [Fact]
    public async Task Create_MessageResponseIsError_ShouldThrowException()
    {
        // Arrange
        var senderIdentity = new DomainIdentity(
            null!,
            null,
            1,
            0,
            2,
            IdentityPoolType.App,
            5,
            "a1",
            2,
            10);
        var recipientIdentity = new DomainIdentity(
            null!,
            new IdentityData
            {
                Address = "identityAddress",
                KeyPair = new KeyPair(publicKey: ConvertibleString.FromUtf8("public-key"), privateKey: ConvertibleString.FromUtf8("private-key"))
            },
            1,
            0,
            3,
            IdentityPoolType.Connector,
            5,
            "c1",
            3,
            20);

        var identities = new List<DomainIdentity>
        {
            senderIdentity,
            recipientIdentity
        };

        var relationshipAndMessages = new List<RelationshipAndMessages>
        {
            new(senderIdentity.PoolAlias, senderIdentity.ConfigurationIdentityAddress, recipientIdentity.PoolAlias, recipientIdentity.ConfigurationIdentityAddress)
            {
                NumberOfSentMessages = senderIdentity.NumberOfSentMessages
            }
        };

        var command = new CreateMessages.Command(
            identities,
            relationshipAndMessages,
            "http://baseurl",
            new ClientCredentials("clientId", "clientSecret"));


        A.CallTo(() => _consumerApiHelper.CreateForExistingIdentity(command.BaseUrlAddress, command.ClientCredentials, senderIdentity.UserCredentials, null))!
            .Returns(_sdkCLient);


        A.CallTo(() => _consumerApiHelper.SendMessage(recipientIdentity, _sdkCLient!))
            .Returns(new ApiResponse<SendMessageResponse>
            {
                Result = new SendMessageResponse
                {
                    Id = "message-",
                    CreatedAt = DateTime.Now
                },
                Error = new ApiError
                {
                    Id = "123",
                    Code = "xyz",
                    Message = "some error message",
                    Docs = "null",
                    Time = DateTime.Now
                }
            });
        // Act
        var act = () => _sut.Create(command, senderIdentity);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();

        A.CallTo(() => _consumerApiHelper.SendMessage(recipientIdentity, _sdkCLient!))
            .MustHaveHappenedOnceOrMore();

        _sut.GetCreateSemaphoreCurrentCount().Should().Be(Environment.ProcessorCount);
        _sut.GetCreateMessagesSemaphoreCurrentCount().Should().Be(Environment.ProcessorCount);
    }

    [Fact]
    public async Task Create_RecipientIdentityAddressIsNull_ShouldThrowException()
    {
        // Arrange
        var senderIdentity = new DomainIdentity(
            null!,
            null,
            1,
            0,
            2,
            IdentityPoolType.App,
            5,
            "a1",
            2,
            10);
        var recipientIdentity = new DomainIdentity(
            null!,
            null,
            1,
            0,
            3,
            IdentityPoolType.Connector,
            5,
            "c1",
            3,
            20);

        var identities = new List<DomainIdentity>
        {
            senderIdentity,
            recipientIdentity
        };

        var relationshipAndMessages = new List<RelationshipAndMessages>
        {
            new(senderIdentity.PoolAlias, senderIdentity.ConfigurationIdentityAddress, recipientIdentity.PoolAlias, recipientIdentity.ConfigurationIdentityAddress)
            {
                NumberOfSentMessages = senderIdentity.NumberOfSentMessages
            }
        };

        var command = new CreateMessages.Command(
            identities,
            relationshipAndMessages,
            "http://baseurl",
            new ClientCredentials("clientId", "clientSecret"));


        A.CallTo(() => _consumerApiHelper.CreateForExistingIdentity(command.BaseUrlAddress, command.ClientCredentials, senderIdentity.UserCredentials, null))!
            .Returns(_sdkCLient);


        A.CallTo(() => _consumerApiHelper.SendMessage(recipientIdentity, _sdkCLient!))
            .Returns(new ApiResponse<SendMessageResponse>
            {
                Result = new SendMessageResponse
                {
                    Id = "message-",
                    CreatedAt = DateTime.Now
                }
            });
        // Act
        var act = () => _sut.Create(command, senderIdentity);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();

        A.CallTo(() => _consumerApiHelper.SendMessage(recipientIdentity, _sdkCLient!))
            .MustNotHaveHappened();

        _sut.GetCreateSemaphoreCurrentCount().Should().Be(Environment.ProcessorCount);
        _sut.GetCreateMessagesSemaphoreCurrentCount().Should().Be(Environment.ProcessorCount);
    }
}
