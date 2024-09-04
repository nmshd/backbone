using Backbone.Tooling;

namespace Backbone.UnitTestTools.BaseClasses;

public abstract class AbstractTestsBase : IDisposable, IAsyncDisposable
{
    protected DateTime _dateTimeNow;
    protected DateTime _dateTimeTomorrow;
    protected DateTime _dateTimeYesterday;

    protected AbstractTestsBase()
    {
        _dateTimeNow = DateTime.UtcNow;
        _dateTimeTomorrow = _dateTimeNow.AddDays(1);
        _dateTimeYesterday = _dateTimeNow.AddDays(-1);
    }

    public virtual void Dispose()
    {
        Task.Run(async () => await DisposeAsync()).GetAwaiter().GetResult();
    }

    public virtual ValueTask DisposeAsync()
    {
        SystemTime.Reset();
        return ValueTask.CompletedTask;
    }
}
