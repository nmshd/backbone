using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Messages.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class IdentityAddress100 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "RecipientInformation",
                type: "char(100)",
                unicode: false,
                fixedLength: true,
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 36);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Messages",
                type: "char(100)",
                unicode: false,
                fixedLength: true,
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 36);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "RecipientInformation",
                type: "char(36)",
                unicode: false,
                fixedLength: true,
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(100)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Messages",
                type: "char(36)",
                unicode: false,
                fixedLength: true,
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(100)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 100);
        }
    }
}
