using AutoFixture;

namespace Messages.Application.Tests.AutoFixture;

public class CustomFixture : Fixture
{
    public CustomFixture()
    {
        Customize(new Customizations());
    }
}
