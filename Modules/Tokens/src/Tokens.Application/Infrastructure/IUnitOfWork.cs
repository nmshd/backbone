namespace Tokens.Application.Infrastructure;

public interface IUnitOfWork : IDisposable
{
    ITokenRepository Tokens { get; }

    Task SaveAsync();
}
