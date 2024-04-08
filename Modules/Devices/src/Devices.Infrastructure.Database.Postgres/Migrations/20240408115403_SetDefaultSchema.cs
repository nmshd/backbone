using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class SetDefaultSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Devices");

            migrationBuilder.RenameTable(
                name: "Tiers",
                newName: "Tiers",
                newSchema: "Devices");

            migrationBuilder.RenameTable(
                name: "PnsRegistrations",
                newName: "PnsRegistrations",
                newSchema: "Devices");

            migrationBuilder.RenameTable(
                name: "OpenIddictTokens",
                newName: "OpenIddictTokens",
                newSchema: "Devices");

            migrationBuilder.RenameTable(
                name: "OpenIddictScopes",
                newName: "OpenIddictScopes",
                newSchema: "Devices");

            migrationBuilder.RenameTable(
                name: "OpenIddictAuthorizations",
                newName: "OpenIddictAuthorizations",
                newSchema: "Devices");

            migrationBuilder.RenameTable(
                name: "OpenIddictApplications",
                newName: "OpenIddictApplications",
                newSchema: "Devices");

            migrationBuilder.RenameTable(
                name: "IdentityDeletionProcesses",
                newName: "IdentityDeletionProcesses",
                newSchema: "Devices");

            migrationBuilder.RenameTable(
                name: "IdentityDeletionProcessAuditLog",
                newName: "IdentityDeletionProcessAuditLog",
                newSchema: "Devices");

            migrationBuilder.RenameTable(
                name: "Identities",
                newName: "Identities",
                newSchema: "Devices");

            migrationBuilder.RenameTable(
                name: "Devices",
                newName: "Devices",
                newSchema: "Devices");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "AspNetUserTokens",
                newSchema: "Devices");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "AspNetUsers",
                newSchema: "Devices");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "AspNetUserRoles",
                newSchema: "Devices");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "AspNetUserLogins",
                newSchema: "Devices");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "AspNetUserClaims",
                newSchema: "Devices");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "AspNetRoles",
                newSchema: "Devices");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "AspNetRoleClaims",
                newSchema: "Devices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Tiers",
                schema: "Devices",
                newName: "Tiers");

            migrationBuilder.RenameTable(
                name: "PnsRegistrations",
                schema: "Devices",
                newName: "PnsRegistrations");

            migrationBuilder.RenameTable(
                name: "OpenIddictTokens",
                schema: "Devices",
                newName: "OpenIddictTokens");

            migrationBuilder.RenameTable(
                name: "OpenIddictScopes",
                schema: "Devices",
                newName: "OpenIddictScopes");

            migrationBuilder.RenameTable(
                name: "OpenIddictAuthorizations",
                schema: "Devices",
                newName: "OpenIddictAuthorizations");

            migrationBuilder.RenameTable(
                name: "OpenIddictApplications",
                schema: "Devices",
                newName: "OpenIddictApplications");

            migrationBuilder.RenameTable(
                name: "IdentityDeletionProcesses",
                schema: "Devices",
                newName: "IdentityDeletionProcesses");

            migrationBuilder.RenameTable(
                name: "IdentityDeletionProcessAuditLog",
                schema: "Devices",
                newName: "IdentityDeletionProcessAuditLog");

            migrationBuilder.RenameTable(
                name: "Identities",
                schema: "Devices",
                newName: "Identities");

            migrationBuilder.RenameTable(
                name: "Devices",
                schema: "Devices",
                newName: "Devices");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                schema: "Devices",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                schema: "Devices",
                newName: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                schema: "Devices",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                schema: "Devices",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                schema: "Devices",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                schema: "Devices",
                newName: "AspNetRoles");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                schema: "Devices",
                newName: "AspNetRoleClaims");
        }
    }
}
