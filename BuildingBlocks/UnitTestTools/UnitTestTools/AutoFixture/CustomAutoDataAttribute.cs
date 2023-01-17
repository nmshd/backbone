using AutoFixture;
using AutoFixture.Xunit2;

namespace Enmeshed.UnitTestTools.AutoFixture
{
    public class CustomAutoDataAttribute : AutoDataAttribute
    {
        public CustomAutoDataAttribute() : base(FixtureFactory)
        {
        }

        private static IFixture FixtureFactory()
        {
            var fixture = new FixtureWithoutRecursion();

            return fixture;
        }
    }
}