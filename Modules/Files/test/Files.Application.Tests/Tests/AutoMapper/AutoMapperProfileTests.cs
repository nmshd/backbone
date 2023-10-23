using AutoMapper;
using Backbone.Files.Application.AutoMapper;
using Xunit;

namespace Backbone.Files.Application.Tests.Tests.AutoMapper;

public class AutoMapperProfileTests
{
    [Fact]
    public void ProfileIsValid()
    {
        // Arrange
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());

        // Act & Assert
        configuration.AssertConfigurationIsValid();
    }
}
