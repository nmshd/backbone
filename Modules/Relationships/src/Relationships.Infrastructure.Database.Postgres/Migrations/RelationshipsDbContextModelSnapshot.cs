﻿// <auto-generated />
using System;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Relationships.Infrastructure.Database.Postgres.Migrations
{
    [DbContext(typeof(RelationshipsDbContext))]
    partial class RelationshipsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Entities.Relationship", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character(20)")
                        .IsFixedLength();

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("From")
                        .IsRequired()
                        .HasMaxLength(36)
                        .IsUnicode(false)
                        .HasColumnType("character(36)")
                        .IsFixedLength();

                    b.Property<string>("RelationshipTemplateId")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character(20)")
                        .IsFixedLength();

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("To")
                        .IsRequired()
                        .HasMaxLength(36)
                        .IsUnicode(false)
                        .HasColumnType("character(36)")
                        .IsFixedLength();

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("From");

                    b.HasIndex("RelationshipTemplateId");

                    b.HasIndex("Status");

                    b.HasIndex("To");

                    b.ToTable("Relationships");
                });

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Entities.RelationshipChange", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RelationshipId")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character(20)")
                        .IsFixedLength();

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("RelationshipId");

                    b.HasIndex("Status");

                    b.HasIndex("Type");

                    b.ToTable("RelationshipChanges", (string)null);

                    b.HasDiscriminator<string>("Discriminator").HasValue("RelationshipChange");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Entities.RelationshipChangeRequest", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<byte[]>("Content")
                        .HasColumnType("bytea")
                        .HasColumnName("Req_Content");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("Req_CreatedAt");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(36)
                        .IsUnicode(false)
                        .HasColumnType("character(36)")
                        .HasColumnName("Req_CreatedBy")
                        .IsFixedLength();

                    b.Property<string>("CreatedByDevice")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character(20)")
                        .HasColumnName("Req_CreatedByDevice")
                        .IsFixedLength();

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedByDevice");

                    b.ToTable("RelationshipChanges", (string)null);
                });

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Entities.RelationshipChangeResponse", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<byte[]>("Content")
                        .HasColumnType("bytea")
                        .HasColumnName("Res_Content");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("Res_CreatedAt");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(36)
                        .IsUnicode(false)
                        .HasColumnType("character(36)")
                        .HasColumnName("Res_CreatedBy")
                        .IsFixedLength();

                    b.Property<string>("CreatedByDevice")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character(20)")
                        .HasColumnName("Res_CreatedByDevice")
                        .IsFixedLength();

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("CreatedByDevice");

                    b.ToTable("RelationshipChanges", (string)null);
                });

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Entities.RelationshipTemplate", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character(20)")
                        .IsFixedLength();

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(36)
                        .IsUnicode(false)
                        .HasColumnType("character(36)")
                        .IsFixedLength();

                    b.Property<string>("CreatedByDevice")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character(20)")
                        .IsFixedLength();

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("ExpiresAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("MaxNumberOfAllocations")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy");

                    b.HasIndex("DeletedAt");

                    b.HasIndex("ExpiresAt");

                    b.ToTable("RelationshipTemplates");
                });

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Entities.RelationshipTemplateAllocation", b =>
                {
                    b.Property<string>("RelationshipTemplateId")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character(20)")
                        .IsFixedLength();

                    b.Property<string>("AllocatedBy")
                        .HasMaxLength(36)
                        .IsUnicode(false)
                        .HasColumnType("character(36)")
                        .IsFixedLength();

                    b.Property<DateTime>("AllocatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("AllocatedByDevice")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character(20)")
                        .IsFixedLength();

                    b.HasKey("RelationshipTemplateId", "AllocatedBy");

                    b.ToTable("RelationshipTemplateAllocations", (string)null);
                });

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Entities.RelationshipCreationChange", b =>
                {
                    b.HasBaseType("Backbone.Modules.Relationships.Domain.Entities.RelationshipChange");

                    b.ToTable("RelationshipChanges", (string)null);

                    b.HasDiscriminator().HasValue("RelationshipCreationChange");
                });

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Entities.RelationshipTerminationChange", b =>
                {
                    b.HasBaseType("Backbone.Modules.Relationships.Domain.Entities.RelationshipChange");

                    b.ToTable("RelationshipChanges", (string)null);

                    b.HasDiscriminator().HasValue("RelationshipTerminationChange");
                });

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Entities.Relationship", b =>
                {
                    b.HasOne("Backbone.Modules.Relationships.Domain.Entities.RelationshipTemplate", null)
                        .WithMany("Relationships")
                        .HasForeignKey("RelationshipTemplateId");
                });

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Entities.RelationshipChange", b =>
                {
                    b.HasOne("Backbone.Modules.Relationships.Domain.Entities.Relationship", "Relationship")
                        .WithMany("Changes")
                        .HasForeignKey("RelationshipId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Relationship");
                });

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Entities.RelationshipChangeRequest", b =>
                {
                    b.HasOne("Backbone.Modules.Relationships.Domain.Entities.RelationshipChange", null)
                        .WithOne("Request")
                        .HasForeignKey("Backbone.Modules.Relationships.Domain.Entities.RelationshipChangeRequest", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Entities.RelationshipChangeResponse", b =>
                {
                    b.HasOne("Backbone.Modules.Relationships.Domain.Entities.RelationshipChange", null)
                        .WithOne("Response")
                        .HasForeignKey("Backbone.Modules.Relationships.Domain.Entities.RelationshipChangeResponse", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Entities.RelationshipTemplateAllocation", b =>
                {
                    b.HasOne("Backbone.Modules.Relationships.Domain.Entities.RelationshipTemplate", null)
                        .WithMany("Allocations")
                        .HasForeignKey("RelationshipTemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Entities.Relationship", b =>
                {
                    b.Navigation("Changes");
                });

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Entities.RelationshipChange", b =>
                {
                    b.Navigation("Request")
                        .IsRequired();

                    b.Navigation("Response");
                });

            modelBuilder.Entity("Backbone.Modules.Relationships.Domain.Entities.RelationshipTemplate", b =>
                {
                    b.Navigation("Allocations");

                    b.Navigation("Relationships");
                });
#pragma warning restore 612, 618
        }
    }
}
