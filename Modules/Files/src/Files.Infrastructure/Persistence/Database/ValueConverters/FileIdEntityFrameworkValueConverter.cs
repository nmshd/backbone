using Backbone.Modules.Files.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Files.Infrastructure.Persistence.Database.ValueConverters;

public class FileIdEntityFrameworkValueConverter : ValueConverter<FileId, string>
{
    public FileIdEntityFrameworkValueConverter() : this(new ConverterMappingHints()) { }

    public FileIdEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            id => id.StringValue,
            value => FileId.Parse(value),
            mappingHints
        )
    { }
}
