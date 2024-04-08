using System.Collections.Concurrent;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.ApplePushNotificationService;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.DirectPush;

public class JwtGeneratorTests : IDisposable
{
    private const string SOME_KEY =
        "MIGHAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBG0wawIBAQQgNsu2YNPiqJQrkibTejrFM2w7D/POivZ4nmoeCwhoPm+hRANCAAToCt8MDybjEhLeCgKh3oAoO7MRT8r041ABrA3uqAXdcAFDhipJeB7SimtCrp2E+QR8qvTvRCMdx3b2srv/UsJZ";

    [Fact]
    public void Generates_a_jwt()
    {
        // Arrange
        var jwtGenerator = CreateJwtGenerator();

        // Act
        var jwt = jwtGenerator.Generate(SOME_KEY, "some-key-id", "some-team-id", "some-bundle-id");

        // Assert
        jwt.Should().NotBeNull();
        jwt.Value.Should().NotBeNull();
    }

    [Fact]
    public void Caches_the_generated_jwt()
    {
        // Arrange
        var jwtGenerator = CreateJwtGenerator();

        var jwt1 = jwtGenerator.Generate(SOME_KEY, "some-key-id", "some-team-id", "some-bundle-id");

        // Act
        var jwt2 = jwtGenerator.Generate(SOME_KEY, "some-key-id", "some-team-id", "some-bundle-id");

        // Assert
        jwt1.Should().BeSameAs(jwt2);
    }

    [Fact]
    public void Generates_new_jwt_for_different_bundle_id()
    {
        // Arrange
        var jwtGenerator = CreateJwtGenerator();

        var jwt1 = jwtGenerator.Generate(SOME_KEY, "some-key-id", "some-team-id", "some-bundle-id");

        // Act
        var jwt2 = jwtGenerator.Generate(SOME_KEY, "some-key-id", "some-team-id", "some-other-bundle-id");

        // Assert
        jwt1.Should().NotBeSameAs(jwt2);
    }

    [Fact]
    public void Renews_the_jwt_after_it_is_expired()
    {
        // Arrange
        var jwtGenerator = CreateJwtGenerator();

        var jwt1 = jwtGenerator.Generate(SOME_KEY, "some-key-id", "some-team-id", "some-bundle-id");
        SystemTime.Set(DateTime.UtcNow.AddMinutes(50));

        // Act
        var jwt2 = jwtGenerator.Generate(SOME_KEY, "some-key-id", "some-team-id", "some-bundle-id");

        // Assert
        jwt1.Should().NotBeSameAs(jwt2);
    }

    [Fact]
    public void Concurrent_executions_generate_only_one_jwt_if_there_is_no_cached_jwt()
    {
        // Arrange
        var jwtGenerator = new JwtGenerator(new ApnsJwtCache());

        var results = new ConcurrentBag<Jwt>();

        // Act
        Parallel.For(0, 10000, _ => { results.Add(jwtGenerator.Generate(SOME_KEY, "some-key-id", "some-team-id", "some-bundle-id")); });

        // Assert
        results.Should().NotBeNull();
        results.Should().HaveCount(10000);

        results.Distinct().Count().Should().Be(1);
    }

    [Fact]
    public void Concurrent_executions_generate_only_one_jwt_if_the_cached_jwt_is_expired()
    {
        // Arrange
        var jwtGenerator = new JwtGenerator(new ApnsJwtCache());

        var initialJwt = jwtGenerator.Generate(SOME_KEY, "some-key-id", "some-team-id", "some-bundle-id");

        var expiredSystemTime = DateTime.UtcNow.AddMinutes(51);

        // Act
        var results = new ConcurrentBag<Jwt>();
        Parallel.For(0, 10000, _ =>
        {
            SystemTime.Set(expiredSystemTime); // we need to set the SystemTime in here, because Parallel executes each iteration in a different thread, and SystemTime sets the time only for the current thread
            results.Add(jwtGenerator.Generate(SOME_KEY, "some-key-id", "some-team-id", "some-bundle-id"));
        });

        // Assert
        results.Should()
            .NotBeNull().And
            .HaveCount(10000);

        results.Count(r => ReferenceEquals(initialJwt, r)).Should().Be(0);

        foreach (var result in results)
        {
            result.Value.Should().NotBeSameAs(initialJwt.Value);
        }
    }

    private static JwtGenerator CreateJwtGenerator()
    {
        return new JwtGenerator(new ApnsJwtCache());
    }

    public void Dispose()
    {
        SystemTime.Reset();
    }
}
