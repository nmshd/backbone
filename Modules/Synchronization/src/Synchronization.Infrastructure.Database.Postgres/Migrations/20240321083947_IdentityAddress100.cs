using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Synchronization.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class IdentityAddress100 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "SyncRuns",
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
                name: "Owner",
                table: "ExternalEvents",
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
                name: "Owner",
                table: "Datawallets",
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
                name: "CreatedBy",
                table: "DatawalletModifications",
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
                name: "CreatedBy",
                table: "SyncRuns",
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
                name: "Owner",
                table: "ExternalEvents",
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
                name: "Owner",
                table: "Datawallets",
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
                name: "CreatedBy",
                table: "DatawalletModifications",
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
