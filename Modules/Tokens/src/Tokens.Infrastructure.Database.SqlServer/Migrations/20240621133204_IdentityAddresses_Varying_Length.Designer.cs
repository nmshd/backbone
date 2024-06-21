﻿// <auto-generated />
using System;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Backbone.Modules.Tokens.Infrastructure.Database.SqlServer.Migrations
{
    [DbContext(typeof(TokensDbContext))]
    [Migration("20240621133204_IdentityAddresses_Varying_Length")]
    partial class IdentityAddresses_Varying_Length
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Tokens")
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Backbone.Modules.Tokens.Domain.Entities.Token", b =>
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

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Tokens", "Tokens");
                });
#pragma warning restore 612, 618
        }
    }
}
