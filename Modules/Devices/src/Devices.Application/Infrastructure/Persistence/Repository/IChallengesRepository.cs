using Backbone.Devices.Domain.Entities;

namespace Backbone.Devices.Application.Infrastructure.Persistence.Repository;

public interface IChallengesRepository
{
    Task<Challenge> FindById(string id, CancellationToken cancellationToken, bool track = false);
}
