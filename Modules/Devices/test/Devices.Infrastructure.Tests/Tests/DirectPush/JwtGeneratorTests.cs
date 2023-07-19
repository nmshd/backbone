using System;
using System.Text.Json;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.ApplePushNotificationService;
using Enmeshed.Tooling;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.DirectPush;

public class JwtGeneratorTests
{
    private const string SOME_KEY =
        "MIGHAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBG0wawIBAQQgNsu2YNPiqJQrkibTejrFM2w7D/POivZ4nmoeCwhoPm+hRANCAAToCt8MDybjEhLeCgKh3oAoO7MRT8r041ABrA3uqAXdcAFDhipJeB7SimtCrp2E+QR8qvTvRCMdx3b2srv/UsJZ";

    [Fact]
    public void Generates_a_jwt()
    {
        var jwtGenerator = new JwtGenerator();
        var jwt = jwtGenerator.Generate(SOME_KEY, "some-key-id", "some-team-id");
        jwt.Should().NotBeNull();
        jwt.Value.Should().NotBeNull();
    }

    [Fact]
    public void Caches_the_generated_jwt()
    {
        var jwtGenerator = new JwtGenerator();
        var jwt1 = jwtGenerator.Generate(SOME_KEY, "some-key-id", "some-team-id");
        var jwt2 = jwtGenerator.Generate(SOME_KEY, "some-key-id", "some-team-id");
        jwt1.Should().BeSameAs(jwt2);
    }

    [Fact]
    public void Renews_the_jwt_after_it_is_expired()
    {
        var jwtGenerator = new JwtGenerator();
        var jwt1 = jwtGenerator.Generate(SOME_KEY, "some-key-id", "some-team-id");
        SystemTime.Set(DateTime.UtcNow.AddMinutes(50));
        var jwt2 = jwtGenerator.Generate(SOME_KEY, "some-key-id", "some-team-id");
        jwt1.Should().NotBeSameAs(jwt2);
    }
}
