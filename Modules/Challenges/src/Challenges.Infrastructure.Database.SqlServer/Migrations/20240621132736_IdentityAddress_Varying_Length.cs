using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Challenges.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class IdentityAddress_Varying_Length : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "Challenges",
                table: "Challenges",
                type: "varchar(80)",
                unicode: false,
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(80)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 80,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "Challenges",
                table: "Challenges",
                type: "char(80)",
                unicode: false,
                fixedLength: true,
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(80)",
                oldUnicode: false,
                oldMaxLength: 80,
                oldNullable: true);
        }
    }
}
