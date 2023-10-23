using AutoMapper;
using Backbone.Relationships.Application.AutoMapper;
using Xunit;

namespace Backbone.Relationships.Application.Tests.Tests.AutoMapper;

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
