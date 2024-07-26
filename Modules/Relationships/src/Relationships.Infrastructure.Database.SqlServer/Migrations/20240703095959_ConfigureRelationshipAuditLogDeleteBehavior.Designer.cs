﻿// <auto-generated />
using System;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Backbone.Modules.Relationships.Infrastructure.Database.SqlServer.Migrations
{
    [DbContext(typeof(RelationshipsDbContext))]
    [Migration("20240703095959_ConfigureRelationshipAuditLogDeleteBehavior")]
    partial class ConfigureRelationshipAuditLogDeleteBehavior
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Relationships")
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates.RelationshipTemplate", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.Property<byte[]>("Content")
                        .HasColumnType("varbinary(max)");

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

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ExpiresAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("MaxNumberOfAllocations")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("RelationshipTemplates", "Relationships");
                });

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates.RelationshipTemplateAllocation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("AllocatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("AllocatedBy")
                        .IsRequired()
                        .HasMaxLength(80)
                        .IsUnicode(false)
                        .HasColumnType("varchar(80)")
                        .IsFixedLength(false);

                    b.Property<string>("AllocatedByDevice")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.Property<string>("RelationshipTemplateId")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.HasKey("Id");

                    b.HasIndex("RelationshipTemplateId", "AllocatedBy");

                    b.ToTable("RelationshipTemplateAllocations", "Relationships");
                });

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Aggregates.Relationships.Relationship", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<byte[]>("CreationContent")
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("CreationResponseContent")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("From")
                        .IsRequired()
                        .HasMaxLength(80)
                        .IsUnicode(false)
                        .HasColumnType("varchar(80)")
                        .IsFixedLength(false);

                    b.Property<bool>("FromHasDecomposed")
                        .HasColumnType("bit");

                    b.Property<string>("RelationshipTemplateId")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("To")
                        .IsRequired()
                        .HasMaxLength(80)
                        .IsUnicode(false)
                        .HasColumnType("varchar(80)")
                        .IsFixedLength(false);

                    b.Property<bool>("ToHasDecomposed")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("From");

                    b.HasIndex("RelationshipTemplateId");

                    b.HasIndex("To");

                    b.ToTable("Relationships", "Relationships");
                });

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Aggregates.Relationships.RelationshipAuditLogEntry", b =>
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

                    b.Property<int>("NewStatus")
                        .HasColumnType("int");

                    b.Property<int?>("OldStatus")
                        .HasColumnType("int");

                    b.Property<int>("Reason")
                        .HasColumnType("int");

                    b.Property<string>("RelationshipId")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.HasKey("Id");

                    b.HasIndex("RelationshipId");

                    b.ToTable("RelationshipAuditLog", "Relationships");
                });

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates.RelationshipTemplateAllocation", b =>
                {
                    b.HasOne("Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates.RelationshipTemplate", null)
                        .WithMany("Allocations")
                        .HasForeignKey("RelationshipTemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Aggregates.Relationships.Relationship", b =>
                {
                    b.HasOne("Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates.RelationshipTemplate", "RelationshipTemplate")
                        .WithMany("Relationships")
                        .HasForeignKey("RelationshipTemplateId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("RelationshipTemplate");
                });

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Aggregates.Relationships.RelationshipAuditLogEntry", b =>
                {
                    b.HasOne("Backbone.Modules.Relationships.Domain.Aggregates.Relationships.Relationship", null)
                        .WithMany("AuditLog")
                        .HasForeignKey("RelationshipId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates.RelationshipTemplate", b =>
                {
                    b.Navigation("Allocations");

                    b.Navigation("Relationships");
                });

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Aggregates.Relationships.Relationship", b =>
                {
                    b.Navigation("AuditLog");
                });
#pragma warning restore 612, 618
        }
    }
}
