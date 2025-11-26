namespace Backbone.BuildingBlocks.Domain;

public class InvalidIdException : Exception
{
    public InvalidIdException(string message) : base(message)
    {
    }
}
