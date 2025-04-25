using MediatR;

namespace Backbone.UnitTestTools.Behaviors;

public class NextMock<TResponse> where TResponse : new()
{
    public NextMock()
    {
        Value = _ =>
        {
            WasCalled = true;
            return Task.FromResult(new TResponse());
        };
    }

    public bool WasCalled { get; set; }
    public RequestHandlerDelegate<TResponse> Value { get; }
}
