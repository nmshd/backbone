namespace Messages.Domain;

public interface Identifiable<out TId>
{
    public TId Id { get; }
}
