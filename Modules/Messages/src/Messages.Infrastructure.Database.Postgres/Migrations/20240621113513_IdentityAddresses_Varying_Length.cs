using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Messages.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class IdentityAddresses_Varying_Length : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "Messages",
                table: "RecipientInformation",
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
                name: "CreatedBy",
                schema: "Messages",
                table: "Messages",
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
                name: "Address",
                schema: "Messages",
                table: "RecipientInformation",
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
                name: "CreatedBy",
                schema: "Messages",
                table: "Messages",
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
