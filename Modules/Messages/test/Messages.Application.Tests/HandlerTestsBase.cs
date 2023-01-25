using AutoFixture;
using AutoMapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;
using Enmeshed.UnitTestTools.TestDoubles.Fakes;
using Messages.Application.AutoMapper;
using Messages.Application.Tests.AutoFixture;
using Messages.Domain.Entities;
using Messages.Infrastructure.Persistence.Database;
using Moq;

namespace Messages.Application.Tests;

public abstract class HandlerTestsBase
{
    protected static readonly IdentityAddress ActiveIdentity = IdentityAddress.Parse("activeidentity");
    protected readonly ApplicationDbContext _actContext;

    protected readonly ApplicationDbContext _arrangeContext;
    protected readonly ApplicationDbContext _assertionContext;
    protected readonly Mock<IEventBus> _eventBusMock;
    protected readonly Fixture _fixture;
    protected readonly IMapper _mapper;
    protected readonly Mock<IUserContext> _userContextMock;
    protected DateTime _dateTimeNow;
    protected DateTime _dateTimeTomorrow;
    protected DateTime _dateTimeYesterday;

    protected HandlerTestsBase()
    {
        _dateTimeNow = DateTime.UtcNow;
        _dateTimeTomorrow = _dateTimeNow.AddDays(1);
        _dateTimeYesterday = _dateTimeNow.AddDays(-1);

        SystemTime.Set(_dateTimeNow);

        _fixture = new CustomFixture();

        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        (_arrangeContext, _assertionContext, _actContext) = FakeDbContextFactory.CreateDbContexts<ApplicationDbContext>();

        _userContextMock = new Mock<IUserContext>();
        _userContextMock.Setup(s => s.GetAddress()).Returns(ActiveIdentity);

        _eventBusMock = new Mock<IEventBus>();

        _mapper = AutoMapperProfile.CreateMapper();
    }

    protected Message AddToDatabase(Message message)
    {
        _arrangeContext.Messages.Add(message);
        _arrangeContext.SaveChanges();
        return message;
    }

    protected Relationship AddToDatabase(Relationship relationship)
    {
        _arrangeContext.Relationships.Add(relationship);
        _arrangeContext.SaveChanges();
        return relationship;
    }
}
