using AutoFixture;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Synchronization.Application.AutoMapper;
using Synchronization.Application.Datawallets.Commands.PushDatawalletModifications;
using Synchronization.Application.Datawallets.DTOs;
using Synchronization.Infrastructure.Persistence.Database;
using Xunit;

namespace Synchronization.Application.Tests.Tests.Datawallet.Commands.PushDatawalletModifications
{
    public class HandlerTests
    {
        private readonly IdentityAddress _activeIdentity = TestDataGenerator.CreateRandomIdentityAddress();
        private readonly DeviceId _activeDevice = TestDataGenerator.CreateRandomDeviceId();
        private readonly DbContextOptions<ApplicationDbContext> _dbOptions;
        private readonly Fixture _testDataGenerator;

        public HandlerTests()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection).Options;

            var setupContext = new ApplicationDbContext(_dbOptions);
            setupContext.Database.EnsureCreated();
            setupContext.Dispose();

            _testDataGenerator = new Fixture();
            _testDataGenerator.Customize<PushDatawalletModificationItem>(composer => composer.With(m => m.DatawalletVersion, 1));
        }

        [Fact]
        public async Task Parallel_push_leads_to_an_error_for_one_call()
        {
            var arrangeContext = CreateDbContext();
            arrangeContext.SaveEntity(new Domain.Entities.Datawallet(new Domain.Entities.Datawallet.DatawalletVersion(1), _activeIdentity));

            // By adding a save-delay to one of the calls, we can ensure that the second one will finish first, and therefore the first one
            // will definitely run into an error regarding the duplicate database index.
            var handlerWithDelayedSave = CreateHandlerWithDelayedSave();
            var handlerWithImmediateSave = CreateHandlerWithImmediateSave();

            var newModifications = _testDataGenerator.CreateMany<PushDatawalletModificationItem>(1).ToArray();


            // Act
            var taskWithImmediateSave = handlerWithDelayedSave.Handle(new PushDatawalletModificationsCommand(newModifications, null, 1), CancellationToken.None);
            var taskWithDelayedSave = handlerWithImmediateSave.Handle(new PushDatawalletModificationsCommand(newModifications, null, 1), CancellationToken.None);

            var handleWithDelayedSave = () => taskWithImmediateSave;
            var handleWithImmediateSave = () => taskWithDelayedSave;


            // Assert
            await handleWithImmediateSave.Should().NotThrowAsync();

            await handleWithDelayedSave
                .Should().ThrowAsync<OperationFailedException>()
                .WithMessage("The sent localIndex does not match the index of the latest modification.*")
                .WithErrorCode("error.platform.validation.datawallet.datawalletNotUpToDate");
        }

        private Handler CreateHandlerWithImmediateSave()
        {
            return CreateHandler(_activeIdentity, _activeDevice, CreateDbContext());
        }

        private ApplicationDbContext CreateDbContext()
        {
            return new ApplicationDbContext(_dbOptions);
        }

        private Handler CreateHandlerWithDelayedSave()
        {
            return CreateHandler(_activeIdentity, _activeDevice, CreateDbContextWithDelayedSave());
        }

        private ApplicationDbContextWithDelayedSave CreateDbContextWithDelayedSave()
        {
            return new ApplicationDbContextWithDelayedSave(_dbOptions, TimeSpan.FromMilliseconds(200));
        }

        private static Handler CreateHandler(IdentityAddress activeIdentity, DeviceId activeDevice, ApplicationDbContext dbContext)
        {
            var userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.GetAddress()).Returns(activeIdentity);
            A.CallTo(() => userContext.GetDeviceId()).Returns(activeDevice);

            var blobStorage = A.Fake<IBlobStorage>();

            var mapper = AutoMapperProfile.CreateMapper();

            var eventBus = A.Fake<IEventBus>();

            return new Handler(dbContext, userContext, mapper, blobStorage, eventBus);
        }
    }
}
