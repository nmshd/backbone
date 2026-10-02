using Backbone.Modules.Devices.Domain.Entities;

namespace Backbone.Modules.Devices.Abstractions;

public interface IChallengesRepository
{
    Task<Challenge?> GetById(string id, CancellationToken cancellationToken, bool track = false);
}
