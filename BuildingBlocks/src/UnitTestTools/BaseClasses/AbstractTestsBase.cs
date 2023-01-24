using AutoFixture;
using Enmeshed.UnitTestTools.AutoFixture;
using Enmeshed.Tooling;

namespace Enmeshed.UnitTestTools.BaseClasses
{
    public abstract class AbstractTestsBase : IDisposable
    {
        protected readonly Fixture _fixture;
        protected DateTime _dateTimeNow;
        protected DateTime _dateTimeTomorrow;
        protected DateTime _dateTimeYesterday;

        protected AbstractTestsBase()
        {
            _dateTimeNow = DateTime.UtcNow;
            _dateTimeTomorrow = _dateTimeNow.AddDays(1);
            _dateTimeYesterday = _dateTimeNow.AddDays(-1);

            SystemTime.Set(_dateTimeNow);

            _fixture = new FixtureWithoutRecursion();
        }

        public virtual void Dispose()
        {
        }
    }
}