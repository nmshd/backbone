using AutoFixture;
using Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;
using Enmeshed.Tooling;

namespace Tokens.Application.Tests.AutoFixture;

public class Customizations : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<CreateTokenCommand>(c => c
            .With(x => x.ExpiresAt, () => SystemTime.UtcNow + fixture.Create<TimeSpan>()));
    }
}
