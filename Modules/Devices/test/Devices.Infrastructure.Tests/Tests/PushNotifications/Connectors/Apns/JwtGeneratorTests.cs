using System.Collections.Concurrent;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors.Apns;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.PushNotifications.Connectors.Apns;

[CollectionDefinition(nameof(JwtGeneratorTests), DisableParallelization = true)]
[Collection(nameof(JwtGeneratorTests))]
public class JwtGeneratorTests : AbstractTestsBase
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
        jwt.ShouldNotBeNull();
        jwt.Value.ShouldNotBeNull();
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
        jwt1.ShouldBeSameAs(jwt2);
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
        jwt1.ShouldNotBeSameAs(jwt2);
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
        jwt1.ShouldNotBeSameAs(jwt2);
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
        results.ShouldNotBeNull();
        results.ShouldHaveCount(10000);

        results.Distinct().Count().ShouldBe(1);
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
            SystemTime.Reset(); // we need to reset the SystemTime after each iteration because resetting it via AbstractTestsBase will only reset the SystemTime for the thread active at that time, leaving the other threads with the wrong time
        });

        // Assert
        results.ShouldNotBeNull();
        results.ShouldHaveCount(10000);

        results.Count(r => ReferenceEquals(initialJwt, r)).ShouldBe(0);

        foreach (var result in results)
        {
            result.Value.ShouldNotBeSameAs(initialJwt.Value);
        }
    }

    private static JwtGenerator CreateJwtGenerator()
    {
        return new JwtGenerator(new ApnsJwtCache());
    }
}
