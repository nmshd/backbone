using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Devices.Domain.Tests.Identities.IdentityDeletionProcessAuditLogEntries;

public class AssociateUsernamesTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var auditLogEntry = IdentityDeletionProcessAuditLogEntry.DeletionCompleted(CreateRandomIdentityAddress());

        // Act
        auditLogEntry.AssociateUsernames([Username.Parse("USR1111111111111111"), Username.Parse("USR2222222222222222")]);

        // Assert
        auditLogEntry.UsernameHashesBase64.ShouldNotBeNull();
        auditLogEntry.UsernameHashesBase64.ShouldHaveCount(2);
        auditLogEntry.UsernameHashesBase64.ShouldContain("jj3azydpiPwK4iFxo/wpCP6pP5Yf5MStnu/hyzMUZ14=", "FQao/LArcVbFzRs4RgYCU5JycRx9zmJMxY5ApJ0Nk8E=");
    }
}
