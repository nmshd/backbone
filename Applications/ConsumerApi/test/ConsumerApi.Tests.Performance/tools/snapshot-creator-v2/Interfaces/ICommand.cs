namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Interfaces;

public interface ICommand<in TArgs, TResult> where TArgs : class
{
    Task<TResult> Execute(TArgs parameter);
}
