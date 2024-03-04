namespace Backbone.BuildingBlocks.Infrastructure.Exceptions;

public class InfrastructureException : Exception
{
    public InfrastructureException(InfrastructureError error) : base(error.Message)
    {
        Code = error.Code;
    }

    public InfrastructureException(InfrastructureError error, Exception innerException) : base(error.Message,
        innerException)
    {
        Code = error.Code;
    }

    public string Code { get; set; }
}
