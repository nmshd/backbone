using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devices.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Change_TierName_Data_Type : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tier",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(30)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 30);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tier",
                type: "character(30)",
                unicode: false,
                fixedLength: true,
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);
        }
    }
}
