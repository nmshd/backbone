﻿// <auto-generated />
using System;
using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Backbone.Modules.Synchronization.Infrastructure.Database.SqlServer.Migrations
{
    [DbContext(typeof(SynchronizationDbContext))]
    [Migration("20241016072720_AddIsDeliveryBlockedAndContextColumnsToExternalEventsTable")]
    partial class AddIsDeliveryBlockedAndContextColumnsToExternalEventsTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Synchronization")
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Backbone.Modules.Synchronization.Domain.Entities.Datawallet", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.Property<string>("Owner")
                        .IsRequired()
                        .HasMaxLength(80)
                        .IsUnicode(false)
                        .HasColumnType("varchar(80)")
                        .IsFixedLength(false);

                    b.Property<ushort>("Version")
                        .IsUnicode(false)
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Owner")
                        .IsUnique();

                    b.ToTable("Datawallets", "Synchronization");
                });

            modelBuilder.Entity("Backbone.Modules.Synchronization.Domain.Entities.DatawalletModification", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.Property<string>("Collection")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(80)
                        .IsUnicode(false)
                        .HasColumnType("varchar(80)")
                        .IsFixedLength(false);

                    b.Property<string>("CreatedByDevice")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.Property<string>("DatawalletId")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.Property<ushort>("DatawalletVersion")
                        .IsUnicode(false)
                        .HasColumnType("int");

                    b.Property<byte[]>("EncryptedPayload")
                        .HasColumnType("varbinary(max)");

                    b.Property<long>("Index")
                        .HasColumnType("bigint");

                    b.Property<string>("ObjectIdentifier")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PayloadCategory")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DatawalletId");

                    b.HasIndex("CreatedBy", "Index")
                        .IsUnique();

                    b.ToTable("DatawalletModifications", "Synchronization");
                });

            modelBuilder.Entity("Backbone.Modules.Synchronization.Domain.Entities.Relationships.Relationship", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Relationships", "Relationships", t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("Backbone.Modules.Synchronization.Domain.Entities.Sync.ExternalEvent", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.Property<string>("Context")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<long>("Index")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsDeliveryBlocked")
                        .HasColumnType("bit");

                    b.Property<string>("Owner")
                        .IsRequired()
                        .HasMaxLength(80)
                        .IsUnicode(false)
                        .HasColumnType("varchar(80)")
                        .IsFixedLength(false);

                    b.Property<string>("Payload")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<byte>("SyncErrorCount")
                        .HasColumnType("tinyint");

                    b.Property<string>("SyncRunId")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.Property<int>("Type")
                        .HasMaxLength(50)
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SyncRunId");

                    b.HasIndex("Owner", "Index")
                        .IsUnique();

                    b.HasIndex("Owner", "SyncRunId");

                    b.ToTable("ExternalEvents", "Synchronization");
                });

            modelBuilder.Entity("Backbone.Modules.Synchronization.Domain.Entities.Sync.SyncError", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.Property<string>("ErrorCode")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ExternalEventId")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.Property<string>("SyncRunId")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.HasKey("Id");

                    b.HasIndex("ExternalEventId");

                    b.HasIndex("SyncRunId", "ExternalEventId")
                        .IsUnique();

                    b.ToTable("SyncErrors", "Synchronization");
                });

            modelBuilder.Entity("Backbone.Modules.Synchronization.Domain.Entities.Sync.SyncRun", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(80)
                        .IsUnicode(false)
                        .HasColumnType("varchar(80)")
                        .IsFixedLength(false);

                    b.Property<string>("CreatedByDevice")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.Property<int>("EventCount")
                        .HasColumnType("int");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FinalizedAt")
                        .HasColumnType("datetime2");

                    b.Property<long>("Index")
                        .HasColumnType("bigint");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy", "FinalizedAt");

                    b.HasIndex("CreatedBy", "Index")
                        .IsUnique();

                    b.ToTable("SyncRuns", "Synchronization");
                });

            modelBuilder.Entity("Backbone.Modules.Synchronization.Domain.Entities.DatawalletModification", b =>
                {
                    b.HasOne("Backbone.Modules.Synchronization.Domain.Entities.Datawallet", "Datawallet")
                        .WithMany("Modifications")
                        .HasForeignKey("DatawalletId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Datawallet");
                });

            modelBuilder.Entity("Backbone.Modules.Synchronization.Domain.Entities.Sync.ExternalEvent", b =>
                {
                    b.HasOne("Backbone.Modules.Synchronization.Domain.Entities.Sync.SyncRun", "SyncRun")
                        .WithMany("ExternalEvents")
                        .HasForeignKey("SyncRunId");

                    b.Navigation("SyncRun");
                });

            modelBuilder.Entity("Backbone.Modules.Synchronization.Domain.Entities.Sync.SyncError", b =>
                {
                    b.HasOne("Backbone.Modules.Synchronization.Domain.Entities.Sync.ExternalEvent", null)
                        .WithMany("Errors")
                        .HasForeignKey("ExternalEventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backbone.Modules.Synchronization.Domain.Entities.Sync.SyncRun", null)
                        .WithMany("Errors")
                        .HasForeignKey("SyncRunId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Backbone.Modules.Synchronization.Domain.Entities.Datawallet", b =>
                {
                    b.Navigation("Modifications");
                });

            modelBuilder.Entity("Backbone.Modules.Synchronization.Domain.Entities.Sync.ExternalEvent", b =>
                {
                    b.Navigation("Errors");
                });

            modelBuilder.Entity("Backbone.Modules.Synchronization.Domain.Entities.Sync.SyncRun", b =>
                {
                    b.Navigation("Errors");

                    b.Navigation("ExternalEvents");
                });
#pragma warning restore 612, 618
        }
    }
}
