using Relationships.Jobs.SanityCheck.RelationshipChange.Infrastructure.DataSource;
using Relationships.Jobs.SanityCheck.RelationshipChange.Infrastructure.Reporter;

namespace Relationships.Jobs.SanityCheck.RelationshipChange;

public class Worker : IHostedService
{
    private readonly IHostApplicationLifetime _host;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private IDataSource _dataSource;
    private IReporter _reporter;

    public Worker(IHostApplicationLifetime host, IServiceScopeFactory serviceScopeFactory)
    {
        _host = host;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        _dataSource = scope.ServiceProvider.GetRequiredService<IDataSource>();
        _reporter = scope.ServiceProvider.GetRequiredService<IReporter>();

        await RunSanityCheck(cancellationToken);

        _host.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task RunSanityCheck(CancellationToken cancellationToken)
    {
        var sanityCheck = new Infrastructure.SanityCheck.SanityCheck(_dataSource, _reporter);

        await sanityCheck.Run(cancellationToken);
    }
}
