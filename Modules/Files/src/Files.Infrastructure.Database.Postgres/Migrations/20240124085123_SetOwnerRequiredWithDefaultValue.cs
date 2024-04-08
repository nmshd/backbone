using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Files.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class SetOwnerRequiredWithDefaultValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                table: "FileMetadata",
                type: "character(36)",
                unicode: false,
                fixedLength: true,
                maxLength: 36,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character(36)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 36,
                oldNullable: true);
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
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character(36)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 36);
        }
    }
}
