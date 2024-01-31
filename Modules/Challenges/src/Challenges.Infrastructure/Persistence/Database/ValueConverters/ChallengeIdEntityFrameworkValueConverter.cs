using Backbone.Modules.Challenges.Domain.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Challenges.Infrastructure.Persistence.Database.ValueConverters;

public class ChallengeIdEntityFrameworkValueConverter : ValueConverter<ChallengeId, string>
{
    public ChallengeIdEntityFrameworkValueConverter() : this(null) { }

    public ChallengeIdEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            id => id.StringValue,
            value => ChallengeId.Parse(value),
            mappingHints
        )
    { }
}
