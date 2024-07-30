using Backbone.Modules.Files.Application.Files.Queries.GetFileContent;
using Backbone.Modules.Files.Domain.Entities;
using Backbone.UnitTestTools.BaseClasses;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Backbone.Modules.Files.Application.Tests.Tests.Files.Queries.GetFileContent;

public class ValidatorTests : AbstractTestsBase
{
    [Fact]
    public void Happy_path()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new GetFileContentQuery { Id = FileId.New() });

        validationResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_when_file_id_is_invalid()
    {
        var validator = new Validator();

        var validationResult = validator.TestValidate(new GetFileContentQuery { Id = "some-invalid-file-id" });

        validationResult.ShouldHaveValidationErrorFor(x => x.Id);
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.First().ErrorCode.Should().Be("error.platform.validation.invalidPropertyValue");
        validationResult.Errors.First().ErrorMessage.Should().Be("The ID is not valid. Check length, prefix and the used characters.");
    }
}
