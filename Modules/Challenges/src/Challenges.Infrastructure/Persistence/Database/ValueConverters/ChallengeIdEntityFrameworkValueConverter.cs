﻿using Backbone.Challenges.Domain.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Challenges.Infrastructure.Persistence.Database.ValueConverters;

public class ChallengeIdEntityFrameworkValueConverter : ValueConverter<ChallengeId, string>
{
    public ChallengeIdEntityFrameworkValueConverter() : this(null) { }

    public ChallengeIdEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            id => id == null ? null : id.StringValue,
            value => ChallengeId.Parse(value),
            mappingHints
        )
    { }
}
