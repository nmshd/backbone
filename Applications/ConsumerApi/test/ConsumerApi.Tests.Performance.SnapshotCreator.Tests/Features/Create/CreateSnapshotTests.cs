using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Helper;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using FakeItEasy;
using MediatR;
using Microsoft.Extensions.Logging;
using FileNotFoundException = System.IO.FileNotFoundException;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Create;

public class CreateSnapshotTests : SnapshotCreatorTestsBase
{
    private readonly IPoolConfigurationJsonReader _poolConfigurationJsonReader;
    private readonly IMediator _mediator;
    private readonly IOutputHelper _outputHelper;
    private readonly CreateSnapshot.CommandHandler _sut;

    public CreateSnapshotTests()
    {
        var logger = A.Fake<ILogger<CreateSnapshot.CommandHandler>>();
        _poolConfigurationJsonReader = A.Fake<IPoolConfigurationJsonReader>();
        _mediator = A.Fake<IMediator>();
        _outputHelper = A.Fake<IOutputHelper>();
        var databaseRestoreHelper = A.Fake<IDatabaseRestoreHelper>();
        _sut = new CreateSnapshot.CommandHandler(logger, _poolConfigurationJsonReader, _mediator, _outputHelper, databaseRestoreHelper);
    }

    [Theory]
    [InlineData("pool-config.test.json")]
    [InlineData("pool-config.light.json")]
    [InlineData("pool-config.heavy.json")]
    public async Task Handle_ShouldReturnSuccessStatusMessage_WhenPoolConfigurationIsCreatedSuccessfully(string poolConfigJsonFilename)
    {
        // Arrange
        var fullFilePath = GetFullFilePath(poolConfigJsonFilename);
        var command = new CreateSnapshot.Command("http://baseaddress", "clientId", "clientSecret", fullFilePath, ClearDatabase: false, BackupDatabase: false, ClearOnly: false);

        A.CallTo(() => _poolConfigurationJsonReader.Read(command.JsonFilePath)).Returns(
            new PerformanceTestConfiguration(
                new List<PoolConfiguration>(),
                new VerificationConfiguration()));

        var domainIdentities = new List<DomainIdentity>();

        A.CallTo(() => _mediator.Send(A<CreateIdentities.Command>._, A<CancellationToken>._)).Returns(domainIdentities);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _poolConfigurationJsonReader.Read(command.JsonFilePath)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _mediator.Send(A<CreateIdentities.Command>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _mediator.Send(A<CreateDevices.Command>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _outputHelper.WriteIdentities(A<string>._, A<List<DomainIdentity>>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _mediator.Send(A<CreateChallenges.Command>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _outputHelper.WriteChallenges(A<string>._, A<List<DomainIdentity>>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _mediator.Send(A<CreateDatawalletModifications.Command>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _outputHelper.WriteDatawalletModifications(A<string>._, A<List<DomainIdentity>>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _mediator.Send(A<CreateRelationshipTemplates.Command>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _outputHelper.WriteRelationshipTemplates(A<string>._, A<List<DomainIdentity>>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _mediator.Send(A<CreateRelationships.Command>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _outputHelper.WriteRelationships(A<string>._, A<List<DomainIdentity>>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _mediator.Send(A<CreateMessages.Command>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _outputHelper.WriteMessages(A<string>._, A<List<DomainIdentity>>._)).MustHaveHappenedOnceExactly();

        result.Should().BeEquivalentTo(new StatusMessage(true, SNAPSHOT_CREATION_SUCCEED_MESSAGE));

        if (Directory.Exists(_sut.OutputDirName))
        {
            Directory.Delete(_sut.OutputDirName, true);
        }
    }

    [Theory]
    [InlineData("pool-config.test.json")]
    [InlineData("pool-config.light.json")]
    [InlineData("pool-config.heavy.json")]
    public async Task Handle_ShouldReturnFailureStatusMessage_WhenPoolConfigurationIsNull(string poolConfigJsonFilename)
    {
        // Arrange
        var fullFilePath = GetFullFilePath(poolConfigJsonFilename);
        var command = new CreateSnapshot.Command("http://baseaddress", "clientId", "clientSecret", fullFilePath, ClearDatabase: false, BackupDatabase: false, ClearOnly: false);

        A.CallTo(() => _poolConfigurationJsonReader.Read(command.JsonFilePath)).Returns(null as PerformanceTestConfiguration);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(new StatusMessage(false, POOL_CONFIG_FILE_READ_ERROR));

        if (Directory.Exists(_sut.OutputDirName))
        {
            Directory.Delete(_sut.OutputDirName, true);
        }
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureStatusMessage_WhenExceptionIsThrown()
    {
        // Arrange
        var command = new CreateSnapshot.Command("http://baseaddress", "clientId", "clientSecret", GetFullFilePath("pool-config.test.json"), ClearDatabase: false, BackupDatabase: false,
            ClearOnly: false);
        var expectedException = new Exception("some exception");
        A.CallTo(() => _poolConfigurationJsonReader.Read(command.JsonFilePath)).ThrowsAsync(expectedException);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(new StatusMessage(false, expectedException.Message, expectedException));

        if (Directory.Exists(_sut.OutputDirName))
        {
            Directory.Delete(_sut.OutputDirName, true);
        }
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureStatusMessage_WhenPoolConfigurationFileNotFound()
    {
        // Arrange
        var fullFilePath = GetFullFilePath("not-existing-pool-config.test.json");
        var command = new CreateSnapshot.Command("http://baseaddress", "clientId", "clientSecret", fullFilePath, ClearDatabase: false, BackupDatabase: false, ClearOnly: false);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Message.Should().Be(POOL_CONFIG_FILE_NOT_FOUND_ERROR);
        result.Status.Should().BeFalse();
        result.Exception.Should().BeOfType<FileNotFoundException>();

        if (Directory.Exists(_sut.OutputDirName))
        {
            Directory.Delete(_sut.OutputDirName, true);
        }
    }
}
