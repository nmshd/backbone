using AutoMapper;
using Backbone.Modules.Messages.Application.AutoMapper;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Messages.Application.Tests.Tests.AutoMapper;

public class AutoMapperProfileTests
{
    [Fact]
    public void ProfileIsValid()
    {
        // Arrange
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());

        // Act & Assert
        configuration.AssertConfigurationIsValid();
        true.Should().BeFalse();
    }
}
