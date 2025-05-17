using System.Linq.Expressions;
using Backbone.Modules.Files.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Files.Infrastructure.Persistence.Database.ValueConverters;

public class FileOwnershipTokenEntityFrameworkValueConverter : ValueConverter<FileOwnershipToken, string>
{
    public FileOwnershipTokenEntityFrameworkValueConverter() : this(new ConverterMappingHints())
    {
    }

    public FileOwnershipTokenEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            id => id.Value,
            value => FileOwnershipToken.Parse(value),
            mappingHints
        )
    {
    }
}
