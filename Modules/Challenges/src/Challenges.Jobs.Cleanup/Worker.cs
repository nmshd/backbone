using Challenges.Application.Challenges.Commands.DeleteExpiredChallenges;
using MediatR;

namespace Challenges.Jobs.Cleanup;

public class Worker : IHostedService
{
    private readonly IHostApplicationLifetime _host;
    private readonly IMediator _mediator;

    public Worker(IHostApplicationLifetime host, IMediator mediator)
    {
        _host = host;
        _mediator = mediator;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteExpiredChallengesCommand(), cancellationToken);

        _host.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
