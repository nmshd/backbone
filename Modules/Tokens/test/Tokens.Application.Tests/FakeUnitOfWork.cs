using Tokens.Application.Infrastructure;

namespace Tokens.Application.Tests;

public class FakeUnitOfWork : IUnitOfWork
{
    public FakeUnitOfWork(ITokenRepository tokenRepository)
    {
        Tokens = tokenRepository;
    }

    public void Dispose() { }

    public ITokenRepository Tokens { get; }

    public Task SaveAsync()
    {
        return Task.CompletedTask;
    }
}
