using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Identity100 : Migration
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

            #region AlterColumns

            migrationBuilder.AlterColumn<string>(
                name: "IdentityAddress",
                schema: "Devices",
                table: "PnsRegistrations",
                type: "char(80)",
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
                name: "IdentityAddress",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
                type: "char(80)",
                unicode: false,
                fixedLength: true,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 36,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "Devices",
                table: "Identities",
                type: "char(80)",
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
                name: "IdentityAddress",
                schema: "Devices",
                table: "Devices",
                type: "char(80)",
                unicode: false,
                fixedLength: true,
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(36)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 36);

            #endregion AlterColumns

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
                principalColumn: "Address");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Identities_IdentityAddress",
                schema: "Devices",
                table: "Devices",
                column: "IdentityAddress",
                principalTable: "Identities",
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

            #region AlterColumns

            migrationBuilder.AlterColumn<string>(
                name: "IdentityAddress",
                schema: "Devices",
                table: "PnsRegistrations",
                type: "char(36)",
                unicode: false,
                fixedLength: true,
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(80)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "IdentityAddress",
                schema: "Devices",
                table: "IdentityDeletionProcesses",
                type: "char(36)",
                unicode: false,
                fixedLength: true,
                maxLength: 36,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(80)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "Devices",
                table: "Identities",
                type: "char(36)",
                unicode: false,
                fixedLength: true,
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(80)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "IdentityAddress",
                schema: "Devices",
                table: "Devices",
                type: "char(36)",
                unicode: false,
                fixedLength: true,
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(80)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 100);

            #endregion AlterColumns

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
                principalColumn: "Address");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Identities_IdentityAddress",
                schema: "Devices",
                table: "Devices",
                column: "IdentityAddress",
                principalTable: "Identities",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
