using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class identity100 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdentityDeletionProcesses_Identities_IdentityAddress",
                table: "IdentityDeletionProcesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Identities_IdentityAddress",
                table: "Devices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Identities",
                table: "Identities");

            #region AlterColumns

            migrationBuilder.AlterColumn<string>(
                name: "IdentityAddress",
                table: "PnsRegistrations",
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
                name: "IdentityAddress",
                table: "IdentityDeletionProcesses",
                type: "char(100)",
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
                table: "Identities",
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
                name: "IdentityAddress",
                table: "Devices",
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

            #endregion AlterColumns

            migrationBuilder.AddPrimaryKey(
                name: "PK_Identities",
                table: "Identities",
                column: "Address");

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityDeletionProcesses_Identities_IdentityAddress",
                table: "IdentityDeletionProcesses",
                column: "IdentityAddress",
                principalTable: "Identities",
                principalColumn: "Address");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Identities_IdentityAddress",
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
               table: "IdentityDeletionProcesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Identities_IdentityAddress",
                table: "Devices");

            migrationBuilder.DropPrimaryKey(
                            name: "PK_Identities",
                            table: "Identities");

            #region AlterColumns

            migrationBuilder.AlterColumn<string>(
                name: "IdentityAddress",
                table: "PnsRegistrations",
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
                name: "IdentityAddress",
                table: "IdentityDeletionProcesses",
                type: "char(36)",
                unicode: false,
                fixedLength: true,
                maxLength: 36,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(100)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Identities",
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
                name: "IdentityAddress",
                table: "Devices",
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

            #endregion AlterColumns

            migrationBuilder.AddPrimaryKey(
                            name: "PK_Identities",
                            table: "Identities",
                            column: "Address");

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityDeletionProcesses_Identities_IdentityAddress",
                table: "IdentityDeletionProcesses",
                column: "IdentityAddress",
                principalTable: "Identities",
                principalColumn: "Address");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Identities_IdentityAddress",
                table: "Devices",
                column: "IdentityAddress",
                principalTable: "Identities",
                principalColumn: "Address",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
