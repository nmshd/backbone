using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Files.Infrastructure.Database.SqlServer.Migrations
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
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OwnershipIsLocked",
                schema: "Files",
                table: "FileMetadata",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnershipToken",
                schema: "Files",
                table: "FileMetadata",
                type: "char(20)",
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
