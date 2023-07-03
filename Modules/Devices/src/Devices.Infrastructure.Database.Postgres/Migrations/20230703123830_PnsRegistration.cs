using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devices.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class PnsRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PnsRegistration",
                columns: table => new
                {
                    Handle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IdentityAddress = table.Column<string>(type: "character(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: true),
                    DeviceId = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PnsRegistration", x => x.Handle);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PnsRegistration");
        }
    }
}
