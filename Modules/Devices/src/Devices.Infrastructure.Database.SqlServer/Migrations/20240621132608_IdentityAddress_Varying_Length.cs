using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class IdentityAddress_Varying_Length : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdentityDeletionProcesses_Identities_IdentityAddress",
                schema: "Devices",
                table: "IdentityDeletionProcesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Identities_IdentityAddress",
                schema: "Devices",
                table: "Devices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Identities",
                schema: "Devices",
                table: "Identities");

            migrationBuilder.AlterColumn<string>(
                name: "IdentityAddress",
                schema: "Devices",
                table: "PnsRegistrations",
                type: "varchar(80)",
                unicode: false,
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(80)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "IdentityAddress",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
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

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "Devices",
                table: "Identities",
                type: "varchar(80)",
                unicode: false,
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(80)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "IdentityAddress",
                schema: "Devices",
                table: "Devices",
                type: "varchar(80)",
                unicode: false,
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(80)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 80);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Identities",
                schema: "Devices",
                table: "Identities",
                column: "Address");

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityDeletionProcesses_Identities_IdentityAddress",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
                column: "IdentityAddress",
                principalTable: "Identities",
                principalSchema: "Devices",
                principalColumn: "Address");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Identities_IdentityAddress",
                schema: "Devices",
                table: "Devices",
                column: "IdentityAddress",
                principalTable: "Identities",
                principalSchema: "Devices",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdentityDeletionProcesses_Identities_IdentityAddress",
                schema: "Devices",
                table: "IdentityDeletionProcesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Identities_IdentityAddress",
                schema: "Devices",
                table: "Devices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Identities",
                schema: "Devices",
                table: "Identities");

            migrationBuilder.AlterColumn<string>(
                name: "IdentityAddress",
                schema: "Devices",
                table: "PnsRegistrations",
                type: "char(80)",
                unicode: false,
                fixedLength: true,
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(80)",
                oldUnicode: false,
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "IdentityAddress",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
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

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "Devices",
                table: "Identities",
                type: "char(80)",
                unicode: false,
                fixedLength: true,
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(80)",
                oldUnicode: false,
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "IdentityAddress",
                schema: "Devices",
                table: "Devices",
                type: "char(80)",
                unicode: false,
                fixedLength: true,
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(80)",
                oldUnicode: false,
                oldMaxLength: 80);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Identities",
                schema: "Devices",
                table: "Identities",
                column: "Address");

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityDeletionProcesses_Identities_IdentityAddress",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
                column: "IdentityAddress",
                principalTable: "Identities",
                principalSchema: "Devices",
                principalColumn: "Address");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Identities_IdentityAddress",
                schema: "Devices",
                table: "Devices",
                column: "IdentityAddress",
                principalTable: "Identities",
                principalSchema: "Devices",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
