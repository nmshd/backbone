using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;

namespace Backbone.Modules.Tokens.Application.Infrastructure;

public interface IUnitOfWork : IDisposable
{
    ITokenRepository Tokens { get; }

    Task SaveAsync();
}
