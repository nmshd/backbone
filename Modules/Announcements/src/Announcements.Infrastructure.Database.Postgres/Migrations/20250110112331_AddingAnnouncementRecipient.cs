using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Announcements.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddingAnnouncementRecipient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnnouncementRecipients",
                schema: "Announcements",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AnnouncementId1 = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnouncementRecipients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnouncementRecipients_Announcements_AnnouncementId1",
                        column: x => x.AnnouncementId1,
                        principalSchema: "Announcements",
                        principalTable: "Announcements",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DomainEvent",
                schema: "Announcements",
                columns: table => new
                {
                    DomainEventId = table.Column<string>(type: "text", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AnnouncementRecipientId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainEvent", x => x.DomainEventId);
                    table.ForeignKey(
                        name: "FK_DomainEvent_AnnouncementRecipients_AnnouncementRecipientId",
                        column: x => x.AnnouncementRecipientId,
                        principalSchema: "Announcements",
                        principalTable: "AnnouncementRecipients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementRecipients_AnnouncementId1",
                schema: "Announcements",
                table: "AnnouncementRecipients",
                column: "AnnouncementId1");

            migrationBuilder.CreateIndex(
                name: "IX_DomainEvent_AnnouncementRecipientId",
                schema: "Announcements",
                table: "DomainEvent",
                column: "AnnouncementRecipientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DomainEvent",
                schema: "Announcements");

            migrationBuilder.DropTable(
                name: "AnnouncementRecipients",
                schema: "Announcements");
        }
    }
}
