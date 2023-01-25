using AutoFixture;

namespace Tokens.Application.Tests.AutoFixture;

public class CustomFixture : Fixture
{
    public CustomFixture()
    {
        Customize(new Customizations());
    }
}
