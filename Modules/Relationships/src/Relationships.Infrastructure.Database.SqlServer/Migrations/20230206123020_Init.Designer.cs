// <auto-generated />
using System;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Relationships.Infrastructure.Database.SqlServer.Migrations
{
    [DbContext(typeof(RelationshipsDbContext))]
    [Migration("20220929131612_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Relationships")
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Relationships.Domain.Entities.Relationship", b =>
            {
                b.Property<string>("Id")
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnType("char(20)")
                    .IsFixedLength();

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("From")
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnType("char(36)")
                    .IsFixedLength();

                b.Property<string>("RelationshipTemplateId")
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnType("char(20)")
                    .IsFixedLength();

                b.Property<int>("Status")
                    .HasColumnType("int");

                b.Property<string>("To")
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnType("char(36)")
                    .IsFixedLength();

                b.HasKey("Id");

                b.HasIndex("CreatedAt");

                b.HasIndex("From");

                b.HasIndex("RelationshipTemplateId");

                b.HasIndex("Status");

                b.HasIndex("To");

                b.ToTable("Relationships", "Relationships");
            });

            modelBuilder.Entity("Relationships.Domain.Entities.RelationshipChange", b =>
            {
                b.Property<string>("Id")
                    .HasMaxLength(20)
                    .HasColumnType("nvarchar(20)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("Discriminator")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("RelationshipId")
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnType("char(20)")
                    .IsFixedLength();

                b.Property<int>("Status")
                    .HasColumnType("int");

                b.Property<int>("Type")
                    .HasColumnType("int");

                b.HasKey("Id");

                b.HasIndex("CreatedAt");

                b.HasIndex("RelationshipId");

                b.HasIndex("Status");

                b.HasIndex("Type");

                b.ToTable("RelationshipChanges", "Relationships");

                b.HasDiscriminator<string>("Discriminator").HasValue("RelationshipChange");
            });

            modelBuilder.Entity("Relationships.Domain.Entities.RelationshipChangeRequest", b =>
            {
                b.Property<string>("Id")
                    .HasMaxLength(20)
                    .HasColumnType("nvarchar(20)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("Req_CreatedAt");

                b.Property<string>("CreatedBy")
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnType("char(36)")
                    .HasColumnName("Req_CreatedBy")
                    .IsFixedLength();

                b.Property<string>("CreatedByDevice")
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnType("char(20)")
                    .HasColumnName("Req_CreatedByDevice")
                    .IsFixedLength();

                b.HasKey("Id");

                b.HasIndex("CreatedAt");

                b.HasIndex("CreatedBy");

                b.HasIndex("CreatedByDevice");

                b.ToTable("RelationshipChanges", "Relationships");
            });

            modelBuilder.Entity("Relationships.Domain.Entities.RelationshipChangeResponse", b =>
            {
                b.Property<string>("Id")
                    .HasMaxLength(20)
                    .HasColumnType("nvarchar(20)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2")
                    .HasColumnName("Res_CreatedAt");

                b.Property<string>("CreatedBy")
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnType("char(36)")
                    .HasColumnName("Res_CreatedBy")
                    .IsFixedLength();

                b.Property<string>("CreatedByDevice")
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnType("char(20)")
                    .HasColumnName("Res_CreatedByDevice")
                    .IsFixedLength();

                b.HasKey("Id");

                b.HasIndex("CreatedAt");

                b.HasIndex("CreatedBy");

                b.HasIndex("CreatedByDevice");

                b.ToTable("RelationshipChanges", "Relationships");
            });

            modelBuilder.Entity("Relationships.Domain.Entities.RelationshipTemplate", b =>
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
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnType("char(36)")
                    .IsFixedLength();

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

                b.HasIndex("CreatedBy");

                b.HasIndex("DeletedAt");

                b.HasIndex("ExpiresAt");

                b.ToTable("RelationshipTemplates", "Relationships");
            });

            modelBuilder.Entity("Relationships.Domain.Entities.RelationshipTemplateAllocation", b =>
            {
                b.Property<string>("RelationshipTemplateId")
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnType("char(20)")
                    .IsFixedLength();

                b.Property<string>("AllocatedBy")
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnType("char(36)")
                    .IsFixedLength();

                b.Property<DateTime>("AllocatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("AllocatedByDevice")
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnType("char(20)")
                    .IsFixedLength();

                b.HasKey("RelationshipTemplateId", "AllocatedBy");

                b.ToTable("RelationshipTemplateAllocations", "Relationships");
            });

            modelBuilder.Entity("Relationships.Domain.Entities.RelationshipCreationChange", b =>
            {
                b.HasBaseType("Relationships.Domain.Entities.RelationshipChange");

                b.ToTable("RelationshipChanges", "Relationships");

                b.HasDiscriminator().HasValue("RelationshipCreationChange");
            });

            modelBuilder.Entity("Relationships.Domain.Entities.RelationshipTerminationChange", b =>
            {
                b.HasBaseType("Relationships.Domain.Entities.RelationshipChange");

                b.ToTable("RelationshipChanges", "Relationships");

                b.HasDiscriminator().HasValue("RelationshipTerminationChange");
            });

            modelBuilder.Entity("Relationships.Domain.Entities.Relationship", b =>
            {
                b.HasOne("Relationships.Domain.Entities.RelationshipTemplate", null)
                    .WithMany("Relationships")
                    .HasForeignKey("RelationshipTemplateId");
            });

            modelBuilder.Entity("Relationships.Domain.Entities.RelationshipChange", b =>
            {
                b.HasOne("Relationships.Domain.Entities.Relationship", "Relationship")
                    .WithMany("Changes")
                    .HasForeignKey("RelationshipId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Relationship");
            });

            modelBuilder.Entity("Relationships.Domain.Entities.RelationshipChangeRequest", b =>
            {
                b.HasOne("Relationships.Domain.Entities.RelationshipChange", null)
                    .WithOne("Request")
                    .HasForeignKey("Relationships.Domain.Entities.RelationshipChangeRequest", "Id")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Relationships.Domain.Entities.RelationshipChangeResponse", b =>
            {
                b.HasOne("Relationships.Domain.Entities.RelationshipChange", null)
                    .WithOne("Response")
                    .HasForeignKey("Relationships.Domain.Entities.RelationshipChangeResponse", "Id")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Relationships.Domain.Entities.RelationshipTemplateAllocation", b =>
            {
                b.HasOne("Relationships.Domain.Entities.RelationshipTemplate", null)
                    .WithMany("Allocations")
                    .HasForeignKey("RelationshipTemplateId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Relationships.Domain.Entities.Relationship", b =>
            {
                b.Navigation("Changes");
            });

            modelBuilder.Entity("Relationships.Domain.Entities.RelationshipChange", b =>
            {
                b.Navigation("Request")
                    .IsRequired();

                b.Navigation("Response");
            });

            modelBuilder.Entity("Relationships.Domain.Entities.RelationshipTemplate", b =>
            {
                b.Navigation("Allocations");

                b.Navigation("Relationships");
            });
#pragma warning restore 612, 618
        }
    }
}
