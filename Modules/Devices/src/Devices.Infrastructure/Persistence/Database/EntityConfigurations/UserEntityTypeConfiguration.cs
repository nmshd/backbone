using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Database.EntityConfigurations;

internal class UserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder
            .Ignore(x => x.Email)
            .Ignore(x => x.EmailConfirmed)
            .Ignore(x => x.HasLoggedIn)
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
