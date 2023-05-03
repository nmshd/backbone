using Backbone.Modules.Tokens.Application.Infrastructure;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;

namespace Backbone.Modules.Tokens.Application.Tests;

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
