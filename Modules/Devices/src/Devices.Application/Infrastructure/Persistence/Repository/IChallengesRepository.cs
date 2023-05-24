using Backbone.Modules.Devices.Domain.Entities;

namespace Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

public interface IChallengesRepository
{
    Task<Challenge> FindById(string id, CancellationToken cancellationToken, bool track = false);
}
