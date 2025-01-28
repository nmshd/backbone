﻿// <auto-generated />
using System;
using Backbone.Modules.Announcements.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Backbone.Modules.Announcements.Infrastructure.Database.SqlServer.Migrations
{
    [DbContext(typeof(AnnouncementsDbContext))]
    partial class AnnouncementsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Announcements")
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Backbone.Modules.Announcements.Domain.Entities.Announcement", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ExpiresAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Severity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Announcements", "Announcements");
                });

            modelBuilder.Entity("Backbone.Modules.Announcements.Domain.Entities.AnnouncementRecipient", b =>
                {
                    b.Property<string>("AnnouncementId")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.Property<string>("Address")
                        .HasMaxLength(80)
                        .IsUnicode(false)
                        .HasColumnType("varchar(80)")
                        .IsFixedLength(false);

                    b.HasKey("AnnouncementId", "Address");

                    b.HasIndex("AnnouncementId", "Address")
                        .IsUnique();

                    b.ToTable("AnnouncementRecipients", "Announcements");
                });

            modelBuilder.Entity("Backbone.Modules.Announcements.Domain.Entities.AnnouncementText", b =>
                {
                    b.Property<string>("AnnouncementId")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("char(20)")
                        .IsFixedLength();

                    b.Property<string>("Language")
                        .HasMaxLength(2)
                        .IsUnicode(false)
                        .HasColumnType("char(2)")
                        .IsFixedLength();

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AnnouncementId", "Language");

                    b.ToTable("AnnouncementText", "Announcements");
                });

            modelBuilder.Entity("Backbone.Modules.Announcements.Domain.Entities.AnnouncementRecipient", b =>
                {
                    b.HasOne("Backbone.Modules.Announcements.Domain.Entities.Announcement", null)
                        .WithMany("Recipients")
                        .HasForeignKey("AnnouncementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Backbone.Modules.Announcements.Domain.Entities.AnnouncementText", b =>
                {
                    b.HasOne("Backbone.Modules.Announcements.Domain.Entities.Announcement", null)
                        .WithMany("Texts")
                        .HasForeignKey("AnnouncementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Backbone.Modules.Announcements.Domain.Entities.Announcement", b =>
                {
                    b.Navigation("Recipients");

                    b.Navigation("Texts");
                });
#pragma warning restore 612, 618
        }
    }
}
