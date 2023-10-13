﻿// <auto-generated />
using System;
using AdminUi.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AdminUi.Infrastructure.Database.SqlServer.Migrations
{
    [DbContext(typeof(AdminUiDbContext))]
    [Migration("20231013091005_UseTierDTOInClientsOverview")]
    partial class UseTierDTOInClientsOverview
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AdminUi.Infrastructure.DTOs.ClientOverview", b =>
                {
                    b.Property<string>("ClientId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NumberOfIdentities")
                        .HasColumnType("int");

                    b.HasKey("ClientId");

                    b.ToTable((string)null);

                    b.ToView("ClientOverviews", (string)null);
                });

            modelBuilder.Entity("AdminUi.Infrastructure.DTOs.IdentityOverview", b =>
                {
                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedWithClient")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("DatawalletVersion")
                        .HasColumnType("int");

                    b.Property<byte>("IdentityVersion")
                        .HasColumnType("tinyint");

                    b.Property<DateTime?>("LastLoginAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("NumberOfDevices")
                        .HasColumnType("int");

                    b.Property<string>("TierId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TierName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ToTable((string)null);

                    b.ToView("IdentityOverviews", (string)null);
                });

            modelBuilder.Entity("AdminUi.Infrastructure.DTOs.TierOverview", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NumberOfIdentities")
                        .HasColumnType("int");

                    b.ToTable((string)null);

                    b.ToView("TierOverviews", (string)null);
                });

            modelBuilder.Entity("AdminUi.Infrastructure.DTOs.ClientOverview", b =>
                {
                    b.OwnsOne("AdminUi.Infrastructure.DTOs.TierDTO", "DefaultTier", b1 =>
                        {
                            b1.Property<string>("ClientOverviewClientId")
                                .HasColumnType("nvarchar(450)");

                            b1.Property<string>("Id")
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("DefaultTierId");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("DefaultTierName");

                            b1.HasKey("ClientOverviewClientId");

                            b1.ToTable((string)null);

                            b1.ToView("ClientOverviews");

                            b1.WithOwner()
                                .HasForeignKey("ClientOverviewClientId");
                        });

                    b.Navigation("DefaultTier")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
