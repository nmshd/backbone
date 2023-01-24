using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devices.Domain.Entities;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Devices.Infrastructure.Persistence.Database.EntityConfigurations;

internal class UserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder
            .Ignore(x => x.Email)
            .Ignore(x => x.EmailConfirmed)
            .Ignore(x => x.NormalizedEmail)
            .Ignore(x => x.PhoneNumber)
            .Ignore(x => x.PhoneNumberConfirmed)
            .Ignore(x => x.TwoFactorEnabled);
        
        builder
            .Property(x => x.UserName)
            .HasMaxLength(Username.MAX_LENGTH)
            .IsFixedLength()
            .IsUnicode(false);

    }
}
