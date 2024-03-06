namespace Backbone.BuildingBlocks.Domain;

public class InvalidIdException : Exception
{
    private const string DEFAULT_MESSAGE = "The ID is not valid. Check length, prefix and the used characters.";

    public InvalidIdException() : base(DEFAULT_MESSAGE)
    {
    }

    public InvalidIdException(string message) : base(message)
    {
    }

    public InvalidIdException(Exception innerException) : base(DEFAULT_MESSAGE, innerException)
    {
    }

    public InvalidIdException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
