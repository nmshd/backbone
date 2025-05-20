using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Files.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddFileOwnershipToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastOwnershipClaimAt",
                schema: "Files",
                table: "FileMetadata",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OwnershipIsLocked",
                schema: "Files",
                table: "FileMetadata",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnershipToken",
                schema: "Files",
                table: "FileMetadata",
                type: "character(20)",
                unicode: false,
                fixedLength: true,
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastOwnershipClaimAt",
                schema: "Files",
                table: "FileMetadata");

            migrationBuilder.DropColumn(
                name: "OwnershipIsLocked",
                schema: "Files",
                table: "FileMetadata");

            migrationBuilder.DropColumn(
                name: "OwnershipToken",
                schema: "Files",
                table: "FileMetadata");
        }
    }
}
