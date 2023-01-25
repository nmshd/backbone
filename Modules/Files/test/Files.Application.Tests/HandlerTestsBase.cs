using AutoMapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;
using Enmeshed.UnitTestTools.BaseClasses;
using Files.Application.AutoMapper;
using Files.Domain.Entities;
using Files.Infrastructure.Persistence.Database;
using Moq;

namespace Files.Application.Tests;

public abstract class HandlerTestsBase : RequestHandlerTestsBase<ApplicationDbContext>
{
    protected static readonly IdentityAddress ActiveIdentity = TestData.IdentityAddresses.ADDRESS_1;
    protected static readonly byte[] SomeFileContent = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
    protected readonly Mock<IBlobStorage> _blobStorageMock;
    protected readonly Mock<IEventBus> _eventBusMock;
    protected readonly IMapper _mapper;

    protected readonly Mock<IUserContext> _userContextMock;

    protected HandlerTestsBase()
    {
        SystemTime.Set(_dateTimeNow);

        _eventBusMock = new Mock<IEventBus>();
        _blobStorageMock = new Mock<IBlobStorage>();

        _userContextMock = new Mock<IUserContext>();
        _userContextMock.Setup(s => s.GetAddress()).Returns(ActiveIdentity);

        _mapper = AutoMapperProfile.CreateMapper();
    }

    protected FileMetadata AddToDatabase(FileMetadata fileMetadata)
    {
        _arrangeContext.FileMetadata.Add(fileMetadata);
        _arrangeContext.SaveChanges();
        return fileMetadata;
    }
}
