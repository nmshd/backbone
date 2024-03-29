// <auto-generated />

using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Quotas.Infrastructure.Database.SqlServer.Migrations
{
    [DbContext(typeof(QuotasDbContext))]
    [Migration("20230426134935_Init_Quotas")]
    partial class Init_Quotas
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Entities.Identity", b =>
                {
                    b.Property<string>("Address")
                        .HasMaxLength(36)
                        .IsUnicode(false)
                        .HasColumnType("char(36)")
                        .IsFixedLength();

                    b.Property<string>("TierId")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.HasKey("Address");

                    b.HasIndex("TierId");

                    b.ToTable("Identities");
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Tiers.Tier", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.Property<string>("Name")
                        .HasMaxLength(30)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(30)")
                        .IsFixedLength(false);

                    b.HasKey("Id");

                    b.ToTable("Tiers");
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Entities.Identity", b =>
                {
                    b.HasOne("Backbone.Modules.Quotas.Domain.Aggregates.Tiers.Tier", null)
                        .WithMany()
                        .HasForeignKey("TierId");
                });
#pragma warning restore 612, 618
        }
    }
}
