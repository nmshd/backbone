using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Module.Features.Devices.ListDevices;
using Backbone.UnitTestTools.FluentValidation;
using FluentValidation.TestHelper;
using ListDevicesSlice = Backbone.Modules.Devices.Module.Features.Devices.ListDevices;

namespace Backbone.Modules.Devices.Module.Tests.Features.Devices.ListDevices;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListDevicesSlice.Query { PaginationFilter = new PaginationFilter(), Ids = [DeviceId.New().Value] });

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_device_id_is_invalid()
    {
        // Arrange
        var validator = new Validator();

        // Act
        var validationResult = validator.TestValidate(new ListDevicesSlice.Query { PaginationFilter = new PaginationFilter(), Ids = ["some-invalid-device-id"] });

        // Assert
        validationResult.ShouldHaveValidationErrorForIdInCollection(
            collectionWithInvalidId: nameof(ListDevicesSlice.Query.Ids),
            indexWithInvalidId: 0);
    }
}
