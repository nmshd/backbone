using AutoMapper;
using Backbone.Modules.Synchronization.Application.AutoMapper;
using Xunit;

namespace Synchronization.Application.Tests.Tests.AutoMapper;

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
