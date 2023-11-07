using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Tests;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Database;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Repository;
using Backbone.Tooling;
using Backbone.UnitTestTools.TestDoubles.Fakes;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Extensions.Options;
using Xunit;

namespace Backbone.Modules.Tokens.Application.Tests.Tests;

public class TokenRepositoryTests
{
    private readonly TokensDbContext _arrangeContext;
    private readonly TokensDbContext _actContext;

    public TokenRepositoryTests()
    {
        (_arrangeContext, _actContext, _) = FakeDbContextFactory.CreateDbContexts<TokensDbContext>();
    }

    [Fact]
    public async Task Test()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2000-01-01"));

        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var deviceId = TestDataGenerator.CreateRandomDeviceId();

        var tokenWithContent = new Token(
            identityAddress, 
            deviceId, 
            new byte[] { 1, 2, 3 }, 
            DateTime.UtcNow);

        var tokenWithoutContent = new Token(
            identityAddress,
            deviceId,
            new byte[] { },
            DateTime.UtcNow);

        var existingTokens = new List<Token>() {
            tokenWithContent,
            tokenWithoutContent
        };

        await _arrangeContext.Tokens.AddRangeAsync(existingTokens);
        await _arrangeContext.SaveChangesAsync();

        var fakeBlobStorage = A.Fake<IBlobStorage>();

        A.CallTo(() => fakeBlobStorage.FindAsync(
                A<string>._, tokenWithoutContent.Id))
            .Returns(new byte[] { 9, 8, 7 });

        var tokenRepository = new TokensRepository(_actContext, fakeBlobStorage, A.Fake<IOptions<TokensRepositoryOptions>>());

        // Act
        var tokens = (await tokenRepository.FindAllWithIds(new TokenId[] { }, new PaginationFilter(), CancellationToken.None))
            .ItemsOnPage.ToList();

        // Assert
        tokens.Count.Should().Be(2);
        tokens[0].Content.Should().BeEquivalentTo(new byte[] { 9, 8, 7 });
        tokens[1].Content.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
    }
}
