using AutoFixture;
using AutoFixture.Dsl;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Files.Application.Tests.Extensions.AutoFixture;

public static class FixtureExtensions
{
    public static ICustomizationComposer<T> BuildWithDefaultCustomizations<T>(this Fixture fixture)
    {
        fixture.Customize<IdentityAddress>(x => x.FromFactory(() => TestData.IdentityAddresses.ADDRESS_1));
        return fixture.Build<T>();
    }

    public static T CreateWithDefaultCustomizations<T>(this Fixture fixture)
    {
        return fixture.BuildWithDefaultCustomizations<T>().Create();
    }
}
