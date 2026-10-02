namespace Backbone.Modules.Devices.Abstractions;

public class OnlyOneActiveDeletionProcessAllowedException : Exception
{
    private const string MESSAGE = "Only one active deletion process is allowed.";

    public OnlyOneActiveDeletionProcessAllowedException(Exception innerException) : base(MESSAGE, innerException)
    {
    }
}
