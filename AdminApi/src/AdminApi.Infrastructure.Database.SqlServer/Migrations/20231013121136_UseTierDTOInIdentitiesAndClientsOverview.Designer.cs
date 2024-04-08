// <auto-generated />
using System;
using Backbone.AdminApi.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AdminUi.Infrastructure.Database.SqlServer.Migrations
{
    [DbContext(typeof(AdminApiDbContext))]
    [Migration("20231013121136_UseTierDTOInIdentitiesAndClientsOverview")]
    partial class UseTierDTOInIdentitiesAndClientsOverview
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("AdminUi")
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
                        .HasColumnType("nvarchar(450)");

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

                    b.HasKey("Address");

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
                                .IsRequired()
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

            modelBuilder.Entity("AdminUi.Infrastructure.DTOs.IdentityOverview", b =>
                {
                    b.OwnsOne("AdminUi.Infrastructure.DTOs.TierDTO", "Tier", b1 =>
                        {
                            b1.Property<string>("IdentityOverviewAddress")
                                .HasColumnType("nvarchar(450)");

                            b1.Property<string>("Id")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("TierId");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("TierName");

                            b1.HasKey("IdentityOverviewAddress");

                            b1.ToTable((string)null);

                            b1.ToView("IdentityOverviews");

                            b1.WithOwner()
                                .HasForeignKey("IdentityOverviewAddress");
                        });

                    b.Navigation("Tier")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
