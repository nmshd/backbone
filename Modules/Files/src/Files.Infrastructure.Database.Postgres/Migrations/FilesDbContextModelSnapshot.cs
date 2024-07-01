﻿// <auto-generated />
using System;
using Backbone.Modules.Files.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backbone.Modules.Files.Infrastructure.Database.Postgres.Migrations
{
    [DbContext(typeof(FilesDbContext))]
    partial class FilesDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Files")
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Backbone.Modules.Files.Domain.Entities.File", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character(20)")
                        .IsFixedLength();

                    b.Property<byte[]>("CipherHash")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<long>("CipherSize")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(80)
                        .IsUnicode(false)
                        .HasColumnType("character varying(80)")
                        .IsFixedLength(false);

                    b.Property<string>("CreatedByDevice")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character(20)")
                        .IsFixedLength();

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DeletedBy")
                        .HasMaxLength(80)
                        .IsUnicode(false)
                        .HasColumnType("character varying(80)")
                        .IsFixedLength(false);

                    b.Property<string>("DeletedByDevice")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character(20)")
                        .IsFixedLength();

                    b.Property<byte[]>("EncryptedProperties")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ModifiedBy")
                        .IsRequired()
                        .HasMaxLength(80)
                        .IsUnicode(false)
                        .HasColumnType("character varying(80)")
                        .IsFixedLength(false);

                    b.Property<string>("ModifiedByDevice")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character(20)")
                        .IsFixedLength();

                    b.Property<string>("Owner")
                        .IsRequired()
                        .HasMaxLength(80)
                        .IsUnicode(false)
                        .HasColumnType("character varying(80)")
                        .IsFixedLength(false);

                    b.Property<byte[]>("OwnerSignature")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.HasKey("Id");

                    b.ToTable("FileMetadata", "Files");
                });
#pragma warning restore 612, 618
        }
    }
}
