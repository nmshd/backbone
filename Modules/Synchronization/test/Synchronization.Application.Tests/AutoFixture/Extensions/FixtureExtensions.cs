using AutoFixture;
using AutoFixture.Dsl;

namespace Synchronization.Application.Tests.AutoFixture.Extensions;

public static class FixtureExtensions
{
    public static ICustomizationComposer<T> BuildWithDefaultCustomizations<T>(this Fixture fixture)
    {
        return fixture.Build<T>();
    }

    public static T CreateWithDefaultCustomizations<T>(this Fixture fixture)
    {
        return fixture.BuildWithDefaultCustomizations<T>().Create();
    }
}
