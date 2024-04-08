// <auto-generated />
using System;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Quotas.Infrastructure.Database.Postgres.Migrations
{
    [DbContext(typeof(QuotasDbContext))]
    [Migration("20230705143449_MetricStatus")]
    partial class MetricStatus
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Quotas")
                .HasAnnotation("ProductVersion", "7.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Identities.Identity", b =>
                {
                    b.Property<string>("Address")
                        .HasMaxLength(36)
                        .IsUnicode(false)
                        .HasColumnType("character(36)")
                        .IsFixedLength();

                    b.Property<string>("TierId")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character(20)")
                        .IsFixedLength();

                    b.HasKey("Address");

                    b.HasIndex("TierId");

                    b.ToTable("Identities");
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Identities.MetricStatus", b =>
                {
                    b.Property<string>("Owner")
                        .HasColumnType("character(36)");

                    b.Property<string>("MetricKey")
                        .HasMaxLength(50)
                        .IsUnicode(true)
                        .HasColumnType("character varying(50)")
                        .IsFixedLength(false);

                    b.Property<DateTime?>("IsExhaustedUntil")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Owner", "MetricKey");

                    b.ToTable("MetricStatus");
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Identities.TierQuota", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character(20)")
                        .IsFixedLength();

                    b.Property<string>("ApplyTo")
                        .HasColumnType("character(36)");

                    b.Property<string>("_definitionId")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character(20)")
                        .HasColumnName("DefinitionId")
                        .IsFixedLength();

                    b.HasKey("Id");

                    b.HasIndex("ApplyTo");

                    b.HasIndex("_definitionId");

                    b.ToTable("TierQuotas");
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Messages.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Messages", "Messages", t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Tiers.Tier", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character(20)")
                        .IsFixedLength();

                    b.Property<string>("Name")
                        .HasMaxLength(30)
                        .IsUnicode(true)
                        .HasColumnType("character varying(30)")
                        .IsFixedLength(false);

                    b.HasKey("Id");

                    b.ToTable("Tiers");
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Tiers.TierQuotaDefinition", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character(20)")
                        .IsFixedLength();

                    b.Property<int>("Max")
                        .HasColumnType("integer");

                    b.Property<string>("MetricKey")
                        .HasMaxLength(50)
                        .IsUnicode(true)
                        .HasColumnType("character varying(50)")
                        .IsFixedLength(false);

                    b.Property<int>("Period")
                        .HasColumnType("integer");

                    b.Property<string>("TierId")
                        .IsUnicode(false)
                        .HasColumnType("character(20)")
                        .IsFixedLength();

                    b.HasKey("Id");

                    b.HasIndex("TierId");

                    b.ToTable("TierQuotaDefinitions");
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Identities.Identity", b =>
                {
                    b.HasOne("Backbone.Modules.Quotas.Domain.Aggregates.Tiers.Tier", null)
                        .WithMany()
                        .HasForeignKey("TierId");
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Identities.MetricStatus", b =>
                {
                    b.HasOne("Backbone.Modules.Quotas.Domain.Aggregates.Identities.Identity", null)
                        .WithMany("MetricStatuses")
                        .HasForeignKey("Owner")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Identities.TierQuota", b =>
                {
                    b.HasOne("Backbone.Modules.Quotas.Domain.Aggregates.Identities.Identity", null)
                        .WithMany("TierQuotas")
                        .HasForeignKey("ApplyTo");

                    b.HasOne("Backbone.Modules.Quotas.Domain.Aggregates.Tiers.TierQuotaDefinition", "_definition")
                        .WithMany()
                        .HasForeignKey("_definitionId");

                    b.Navigation("_definition");
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Tiers.TierQuotaDefinition", b =>
                {
                    b.HasOne("Backbone.Modules.Quotas.Domain.Aggregates.Tiers.Tier", null)
                        .WithMany("Quotas")
                        .HasForeignKey("TierId");
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Identities.Identity", b =>
                {
                    b.Navigation("MetricStatuses");

                    b.Navigation("TierQuotas");
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Tiers.Tier", b =>
                {
                    b.Navigation("Quotas");
                });
#pragma warning restore 612, 618
        }
    }
}
