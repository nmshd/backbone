using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Files.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class IdentityAddress100 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                table: "FileMetadata",
                type: "character(100)",
                unicode: false,
                fixedLength: true,
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(36)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 36);

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "FileMetadata",
                type: "character(100)",
                unicode: false,
                fixedLength: true,
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(36)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 36);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "FileMetadata",
                type: "character(100)",
                unicode: false,
                fixedLength: true,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character(36)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 36,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "FileMetadata",
                type: "character(100)",
                unicode: false,
                fixedLength: true,
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(36)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 36);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                table: "FileMetadata",
                type: "character(36)",
                unicode: false,
                fixedLength: true,
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(100)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "FileMetadata",
                type: "character(36)",
                unicode: false,
                fixedLength: true,
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(100)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "FileMetadata",
                type: "character(36)",
                unicode: false,
                fixedLength: true,
                maxLength: 36,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character(100)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "FileMetadata",
                type: "character(36)",
                unicode: false,
                fixedLength: true,
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(100)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 100);
        }
    }
}
