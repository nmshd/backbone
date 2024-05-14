﻿// <auto-generated />
using System;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Backbone.Modules.Quotas.Infrastructure.Database.SqlServer.Migrations
{
    [DbContext(typeof(QuotasDbContext))]
    [Migration("20240307154323_IdentityAddress80")]
    partial class IdentityAddress80
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Quotas")
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.FileMetadata.FileMetadata", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<long>("CipherSize")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("FileMetadata", "Files", t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Identities.Identity", b =>
                {
                    b.Property<string>("Address")
                        .HasMaxLength(80)
                        .IsUnicode(false)
                        .HasColumnType("char(80)")
                        .IsFixedLength();

                    b.Property<string>("TierId")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.HasKey("Address");

                    b.HasIndex("TierId");

                    b.ToTable("Identities");
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Identities.IndividualQuota", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.Property<string>("ApplyTo")
                        .IsRequired()
                        .HasColumnType("char(80)");

                    b.Property<int>("Max")
                        .HasColumnType("int");

                    b.Property<string>("MetricKey")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(50)")
                        .IsFixedLength(false);

                    b.Property<int>("Period")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ApplyTo");

                    b.ToTable("IndividualQuotas", (string)null);
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Identities.MetricStatus", b =>
                {
                    b.Property<string>("Owner")
                        .HasColumnType("char(80)");

                    b.Property<string>("MetricKey")
                        .HasMaxLength(50)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(50)")
                        .IsFixedLength(false);

                    b.Property<DateTime>("IsExhaustedUntil")
                        .HasColumnType("datetime2");

                    b.HasKey("Owner", "MetricKey");

                    b.ToTable("MetricStatuses", (string)null);
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Identities.TierQuota", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.Property<string>("ApplyTo")
                        .IsRequired()
                        .HasColumnType("char(80)");

                    b.Property<string>("_definitionId")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .HasColumnName("DefinitionId")
                        .IsFixedLength();

                    b.HasKey("Id");

                    b.HasIndex("ApplyTo");

                    b.HasIndex("_definitionId");

                    b.ToTable("TierQuotas", (string)null);
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Messages.Message", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Messages", "Messages", t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Relationships.Relationship", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("From")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("To")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Relationships", "Relationships", t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Relationships.RelationshipTemplate", b =>
                {
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("RelationshipTemplates", "Relationships", t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Tiers.Tier", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(30)")
                        .IsFixedLength(false);

                    b.HasKey("Id");

                    b.ToTable("Tiers");
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Tiers.TierQuotaDefinition", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.Property<int>("Max")
                        .HasColumnType("int");

                    b.Property<string>("MetricKey")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(50)")
                        .IsFixedLength(false);

                    b.Property<int>("Period")
                        .HasColumnType("int");

                    b.Property<string>("TierId")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.HasKey("Id");

                    b.HasIndex("TierId");

                    b.ToTable("TierQuotaDefinitions", (string)null);
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Tokens.Token", b =>
                {
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("Tokens", "Tokens", t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Identities.Identity", b =>
                {
                    b.HasOne("Backbone.Modules.Quotas.Domain.Aggregates.Tiers.Tier", null)
                        .WithMany()
                        .HasForeignKey("TierId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Identities.IndividualQuota", b =>
                {
                    b.HasOne("Backbone.Modules.Quotas.Domain.Aggregates.Identities.Identity", null)
                        .WithMany("IndividualQuotas")
                        .HasForeignKey("ApplyTo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
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
                        .HasForeignKey("ApplyTo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backbone.Modules.Quotas.Domain.Aggregates.Tiers.TierQuotaDefinition", "_definition")
                        .WithMany()
                        .HasForeignKey("_definitionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("_definition");
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Tiers.TierQuotaDefinition", b =>
                {
                    b.HasOne("Backbone.Modules.Quotas.Domain.Aggregates.Tiers.Tier", null)
                        .WithMany("Quotas")
                        .HasForeignKey("TierId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Backbone.Modules.Quotas.Domain.Aggregates.Identities.Identity", b =>
                {
                    b.Navigation("IndividualQuotas");

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
