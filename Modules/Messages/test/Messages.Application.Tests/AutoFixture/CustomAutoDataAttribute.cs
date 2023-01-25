using AutoFixture;
using AutoFixture.Xunit2;

namespace Messages.Application.Tests.AutoFixture;

public class CustomAutoDataAttribute : AutoDataAttribute
{
    public CustomAutoDataAttribute() : base(FixtureFactory) { }

    private static IFixture FixtureFactory()
    {
        var fixture = new CustomFixture();

        fixture.Customize(new Customizations());

        return fixture;
    }
}
