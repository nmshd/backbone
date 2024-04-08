using Backbone.Modules.Synchronization.Application.Datawallets.Commands.PushDatawalletModifications;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.Datawallet.Commands.PushDatawalletModifications;

public class PushDatawalletModificationsCommandValidatorTests
{
    [Fact]
    public void Happy_path()
    {
        var validator = new PushDatawalletModificationsCommandValidator();

        var command = new PushDatawalletModificationsCommand(
            [
                new PushDatawalletModificationItem
                {
                    Collection = "x", DatawalletVersion = 1, EncryptedPayload = [], ObjectIdentifier = "x", PayloadCategory = "x", Type = DatawalletModificationDTO.DatawalletModificationType.Create
                }
            ],
            1);
        var validationResult = validator.TestValidate(command);

        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_not_passing_a_SupportedDatawalletVersion()
    {
        var validator = new PushDatawalletModificationsCommandValidator();

        var command = new PushDatawalletModificationsCommand(
            [
                new PushDatawalletModificationItem
                {
                    Collection = "x", DatawalletVersion = 1, EncryptedPayload = [], ObjectIdentifier = "x", PayloadCategory = "x", Type = DatawalletModificationDTO.DatawalletModificationType.Create
                }
            ],
            0);
        var validationResult = validator.TestValidate(command);

        validationResult.ShouldHaveValidationErrorFor(x => x.SupportedDatawalletVersion);
    }
}
