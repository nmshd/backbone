﻿// <auto-generated />
using System;
using Backbone.Modules.Announcements.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backbone.Modules.Announcements.Infrastructure.Database.Postgres.Migrations
{
    [DbContext(typeof(AnnouncementsDbContext))]
    [Migration("20250115091307_AddAnnouncementRecipientV2")]
    partial class AddAnnouncementRecipientV2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Announcements")
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Backbone.Modules.Announcements.Domain.Entities.Announcement", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character(20)")
                        .IsFixedLength();

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("ExpiresAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Severity")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Announcements", "Announcements");
                });

            modelBuilder.Entity("Backbone.Modules.Announcements.Domain.Entities.AnnouncementRecipient", b =>
                {
                    b.Property<string>("AnnouncementId")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character(20)")
                        .IsFixedLength();

                    b.Property<string>("Address")
                        .HasMaxLength(80)
                        .IsUnicode(false)
                        .HasColumnType("character varying(80)")
                        .IsFixedLength(false);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("AnnouncementId", "Address");

                    b.ToTable("AnnouncementRecipients", "Announcements");
                });

            modelBuilder.Entity("Backbone.Modules.Announcements.Domain.Entities.AnnouncementText", b =>
                {
                    b.Property<string>("AnnouncementId")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character(20)")
                        .IsFixedLength();

                    b.Property<string>("Language")
                        .HasMaxLength(2)
                        .IsUnicode(false)
                        .HasColumnType("character(2)")
                        .IsFixedLength();

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

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
