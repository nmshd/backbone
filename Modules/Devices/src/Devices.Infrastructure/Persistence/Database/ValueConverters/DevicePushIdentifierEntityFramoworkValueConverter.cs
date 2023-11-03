using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backbone.Modules.Devices.Application.Tests.Tests.PushNotifications;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.ValueConverters;
internal class DevicePushIdentifierEntityFramoworkValueConverter : ValueConverter<DevicePushIdentifier, string>
{
    public DevicePushIdentifierEntityFramoworkValueConverter() : this(null) { }

    public DevicePushIdentifierEntityFramoworkValueConverter(ConverterMappingHints mappingHints)
        : base(dpi => dpi.StringValue, 
            value => DevicePushIdentifier.New(),
            mappingHints)
    { }
}
