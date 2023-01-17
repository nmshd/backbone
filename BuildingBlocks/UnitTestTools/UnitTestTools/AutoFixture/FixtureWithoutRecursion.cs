using AutoFixture;

namespace Enmeshed.UnitTestTools.AutoFixture
{
    public class FixtureWithoutRecursion : Fixture
    {
        public FixtureWithoutRecursion()
        {
            Behaviors.Remove(new ThrowingRecursionBehavior());
            Behaviors.Add(new OmitOnRecursionBehavior());
        }
    }
}