using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Files.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class IdentityAddresses_Varying_Length : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                schema: "Files",
                table: "FileMetadata",
                type: "character varying(80)",
                unicode: false,
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(80)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                schema: "Files",
                table: "FileMetadata",
                type: "character varying(80)",
                unicode: false,
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(80)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                schema: "Files",
                table: "FileMetadata",
                type: "character varying(80)",
                unicode: false,
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character(80)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "Files",
                table: "FileMetadata",
                type: "character varying(80)",
                unicode: false,
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(80)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 80);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                schema: "Files",
                table: "FileMetadata",
                type: "character(80)",
                unicode: false,
                fixedLength: true,
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldUnicode: false,
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                schema: "Files",
                table: "FileMetadata",
                type: "character(80)",
                unicode: false,
                fixedLength: true,
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldUnicode: false,
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                schema: "Files",
                table: "FileMetadata",
                type: "character(80)",
                unicode: false,
                fixedLength: true,
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldUnicode: false,
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "Files",
                table: "FileMetadata",
                type: "character(80)",
                unicode: false,
                fixedLength: true,
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldUnicode: false,
                oldMaxLength: 80);
        }
    }
}
