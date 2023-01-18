using Messages.Domain.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Messages.Infrastructure.Persistence.Database.ValueConverters;

public class FileIdEntityFrameworkValueConverter : ValueConverter<FileId, string>
{
    public FileIdEntityFrameworkValueConverter() : this(null) { }

    public FileIdEntityFrameworkValueConverter(ConverterMappingHints mappingHints)
        : base(
            id => id == null ? null : id.StringValue,
            value => FileId.Parse(value),
            mappingHints
        ) { }
}
