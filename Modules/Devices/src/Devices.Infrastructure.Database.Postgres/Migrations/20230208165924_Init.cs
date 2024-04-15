using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Devices.Infrastructure.Database.Postgres.Migrations;

/// <inheritdoc />
public partial class Init : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AspNetRoles",
            schema: "Devices",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetRoles", x => x.Id);
            }
        );

        migrationBuilder.CreateTable(
            name: "Identities",
            schema: "Devices",
            columns: table => new
            {
                Address = table.Column<string>(type: "character(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: false),
                ClientId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                PublicKey = table.Column<byte[]>(type: "bytea", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                IdentityVersion = table.Column<byte>(type: "smallint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Identities", x => x.Address);
            }
        );

        migrationBuilder.CreateTable(
            name: "OpenIddictApplications",
            schema: "Devices",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                ClientId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                ClientSecret = table.Column<string>(type: "text", nullable: true),
                ConcurrencyToken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                ConsentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                DisplayName = table.Column<string>(type: "text", nullable: true),
                DisplayNames = table.Column<string>(type: "text", nullable: true),
                Permissions = table.Column<string>(type: "text", nullable: true),
                PostLogoutRedirectUris = table.Column<string>(type: "text", nullable: true),
                Properties = table.Column<string>(type: "text", nullable: true),
                RedirectUris = table.Column<string>(type: "text", nullable: true),
                Requirements = table.Column<string>(type: "text", nullable: true),
                Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OpenIddictApplications", x => x.Id);
            }
        );

        migrationBuilder.CreateTable(
            name: "OpenIddictScopes",
            schema: "Devices",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                ConcurrencyToken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                Description = table.Column<string>(type: "text", nullable: true),
                Descriptions = table.Column<string>(type: "text", nullable: true),
                DisplayName = table.Column<string>(type: "text", nullable: true),
                DisplayNames = table.Column<string>(type: "text", nullable: true),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                Properties = table.Column<string>(type: "text", nullable: true),
                Resources = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OpenIddictScopes", x => x.Id);
            }
        );

        migrationBuilder.CreateTable(
            name: "AspNetRoleClaims",
            schema: "Devices",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                RoleId = table.Column<string>(type: "text", nullable: false),
                ClaimType = table.Column<string>(type: "text", nullable: true),
                ClaimValue = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                table.ForeignKey(
                    name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                    column: x => x.RoleId,
                    principalSchema: "Devices",
                    principalTable: "AspNetRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            }
        );

        migrationBuilder.CreateTable(
            name: "Devices",
            schema: "Devices",
            columns: table => new
            {
                Id = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                IdentityAddress = table.Column<string>(type: "character(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedByDevice = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                DeletedByDevice = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true),
                DeletionCertificate = table.Column<byte[]>(type: "bytea", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Devices", x => x.Id);
                table.ForeignKey(
                    name: "FK_Devices_Identities_IdentityAddress",
                    column: x => x.IdentityAddress,
                    principalSchema: "Devices",
                    principalTable: "Identities",
                    principalColumn: "Address",
                    onDelete: ReferentialAction.Cascade);
            }
        );

        migrationBuilder.CreateTable(
            name: "OpenIddictAuthorizations",
            schema: "Devices",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                ApplicationId = table.Column<string>(type: "text", nullable: true),
                ConcurrencyToken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                Properties = table.Column<string>(type: "text", nullable: true),
                Scopes = table.Column<string>(type: "text", nullable: true),
                Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                Subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OpenIddictAuthorizations", x => x.Id);
                table.ForeignKey(
                    name: "FK_OpenIddictAuthorizations_OpenIddictApplications_Application~",
                    column: x => x.ApplicationId,
                    principalSchema: "Devices",
                    principalTable: "OpenIddictApplications",
                    principalColumn: "Id");
            }
        );

        migrationBuilder.CreateTable(
            name: "AspNetUsers",
            schema: "Devices",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                DeviceId = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                UserName = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true),
                NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                PasswordHash = table.Column<string>(type: "text", nullable: true),
                SecurityStamp = table.Column<string>(type: "text", nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                table.ForeignKey(
                    name: "FK_AspNetUsers_Devices_DeviceId",
                    column: x => x.DeviceId,
                    principalSchema: "Devices",
                    principalTable: "Devices",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            }
        );

        migrationBuilder.CreateTable(
            name: "OpenIddictTokens",
            schema: "Devices",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                ApplicationId = table.Column<string>(type: "text", nullable: true),
                AuthorizationId = table.Column<string>(type: "text", nullable: true),
                ConcurrencyToken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                Payload = table.Column<string>(type: "text", nullable: true),
                Properties = table.Column<string>(type: "text", nullable: true),
                RedemptionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                ReferenceId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                Subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OpenIddictTokens", x => x.Id);
                table.ForeignKey(
                    name: "FK_OpenIddictTokens_OpenIddictApplications_ApplicationId",
                    column: x => x.ApplicationId,
                    principalSchema: "Devices",
                    principalTable: "OpenIddictApplications",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_OpenIddictTokens_OpenIddictAuthorizations_AuthorizationId",
                    column: x => x.AuthorizationId,
                    principalSchema: "Devices",
                    principalTable: "OpenIddictAuthorizations",
                    principalColumn: "Id");
            }
        );

        migrationBuilder.CreateTable(
            name: "AspNetUserClaims",
            schema: "Devices",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<string>(type: "text", nullable: false),
                ClaimType = table.Column<string>(type: "text", nullable: true),
                ClaimValue = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                table.ForeignKey(
                    name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalSchema: "Devices",
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            }
        );

        migrationBuilder.CreateTable(
            name: "AspNetUserLogins",
            schema: "Devices",
            columns: table => new
            {
                LoginProvider = table.Column<string>(type: "text", nullable: false),
                ProviderKey = table.Column<string>(type: "text", nullable: false),
                ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                UserId = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                table.ForeignKey(
                    name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalSchema: "Devices",
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            }
        );

        migrationBuilder.CreateTable(
            name: "AspNetUserRoles",
            schema: "Devices",
            columns: table => new
            {
                UserId = table.Column<string>(type: "text", nullable: false),
                RoleId = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                table.ForeignKey(
                    name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                    column: x => x.RoleId,
                    principalSchema: "Devices",
                    principalTable: "AspNetRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalSchema: "Devices",
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            }
        );

        migrationBuilder.CreateTable(
            name: "AspNetUserTokens",
            schema: "Devices",
            columns: table => new
            {
                UserId = table.Column<string>(type: "text", nullable: false),
                LoginProvider = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                Value = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                table.ForeignKey(
                    name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalSchema: "Devices",
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            }
        );

        migrationBuilder.CreateIndex(
            name: "IX_AspNetRoleClaims_RoleId",
            schema: "Devices",
            table: "AspNetRoleClaims",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "RoleNameIndex",
            schema: "Devices",
            table: "AspNetRoles",
            column: "NormalizedName",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserClaims_UserId",
            schema: "Devices",
            table: "AspNetUserClaims",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserLogins_UserId",
            schema: "Devices",
            table: "AspNetUserLogins",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserRoles_RoleId",
            schema: "Devices",
            table: "AspNetUserRoles",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUsers_DeviceId",
            schema: "Devices",
            table: "AspNetUsers",
            column: "DeviceId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "UserNameIndex",
            schema: "Devices",
            table: "AspNetUsers",
            column: "NormalizedUserName",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Devices_IdentityAddress",
            schema: "Devices",
            table: "Devices",
            column: "IdentityAddress");

        migrationBuilder.CreateIndex(
            name: "IX_OpenIddictApplications_ClientId",
            schema: "Devices",
            table: "OpenIddictApplications",
            column: "ClientId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_OpenIddictAuthorizations_ApplicationId_Status_Subject_Type",
            schema: "Devices",
            table: "OpenIddictAuthorizations",
            columns: new[] { "ApplicationId", "Status", "Subject", "Type" });

        migrationBuilder.CreateIndex(
            name: "IX_OpenIddictScopes_Name",
            schema: "Devices",
            table: "OpenIddictScopes",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_OpenIddictTokens_ApplicationId_Status_Subject_Type",
            schema: "Devices",
            table: "OpenIddictTokens",
            columns: new[] { "ApplicationId", "Status", "Subject", "Type" });

        migrationBuilder.CreateIndex(
            name: "IX_OpenIddictTokens_AuthorizationId",
            schema: "Devices",
            table: "OpenIddictTokens",
            column: "AuthorizationId");

        migrationBuilder.CreateIndex(
            name: "IX_OpenIddictTokens_ReferenceId",
            schema: "Devices",
            table: "OpenIddictTokens",
            column: "ReferenceId",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AspNetRoleClaims");

        migrationBuilder.DropTable(
            name: "AspNetUserClaims");

        migrationBuilder.DropTable(
            name: "AspNetUserLogins");

        migrationBuilder.DropTable(
            name: "AspNetUserRoles");

        migrationBuilder.DropTable(
            name: "AspNetUserTokens");

        migrationBuilder.DropTable(
            name: "OpenIddictScopes");

        migrationBuilder.DropTable(
            name: "OpenIddictTokens");

        migrationBuilder.DropTable(
            name: "AspNetRoles");

        migrationBuilder.DropTable(
            name: "AspNetUsers");

        migrationBuilder.DropTable(
            name: "OpenIddictAuthorizations");

        migrationBuilder.DropTable(
            name: "Devices");

        migrationBuilder.DropTable(
            name: "OpenIddictApplications");

        migrationBuilder.DropTable(
            name: "Identities");
    }
}
