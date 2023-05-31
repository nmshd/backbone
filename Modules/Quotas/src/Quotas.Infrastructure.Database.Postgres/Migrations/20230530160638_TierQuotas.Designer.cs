﻿// <auto-generated />
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
    [Migration("20230530160638_TierQuotas")]
    partial class TierQuotas
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
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

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Identities.TierQuota", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character(20)")
                        .IsFixedLength();

                    b.Property<string>("ApplyTo")
                        .HasColumnType("character(36)");

                    b.Property<DateTime?>("IsExhaustedUntil")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("_definitionId")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character(20)")
                        .IsFixedLength();

                    b.HasKey("Id");

                    b.HasIndex("ApplyTo");

                    b.HasIndex("_definitionId");

                    b.ToTable("TierQuotas");

                    b.UseTpcMappingStrategy();
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

                    b.Property<int>("MetricKey")
                        .HasColumnType("integer");

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
