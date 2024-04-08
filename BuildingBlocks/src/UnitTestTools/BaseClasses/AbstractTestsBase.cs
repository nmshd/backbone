using Backbone.Tooling;

namespace Backbone.UnitTestTools.BaseClasses;

public abstract class AbstractTestsBase : IDisposable
{
    protected DateTime _dateTimeNow;
    protected DateTime _dateTimeTomorrow;
    protected DateTime _dateTimeYesterday;

    protected AbstractTestsBase()
    {
        _dateTimeNow = DateTime.UtcNow;
        _dateTimeTomorrow = _dateTimeNow.AddDays(1);
        _dateTimeYesterday = _dateTimeNow.AddDays(-1);

        SystemTime.Set(_dateTimeNow);
    }

    public virtual void Dispose()
    {
    }
}
