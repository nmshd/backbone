namespace Messages.Domain;

public interface IIdentifiable<out TId>
{
    public TId Id { get; }
}
