using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Files.Infrastructure.Database.Postgres.Migrations;

/// <inheritdoc />
public partial class Init : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "FileMetadata",
            schema: "Files",
            columns: table => new
            {
                Id = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: false),
                CreatedByDevice = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ModifiedBy = table.Column<string>(type: "character(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: false),
                ModifiedByDevice = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                DeletedBy = table.Column<string>(type: "character(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: true),
                DeletedByDevice = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true),
                Owner = table.Column<string>(type: "character(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: true),
                OwnerSignature = table.Column<byte[]>(type: "bytea", nullable: false),
                CipherSize = table.Column<long>(type: "bigint", nullable: false),
                CipherHash = table.Column<byte[]>(type: "bytea", nullable: false),
                ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                EncryptedProperties = table.Column<byte[]>(type: "bytea", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FileMetadata", x => x.Id);
            }
        );

        migrationBuilder.CreateIndex(
            name: "IX_FileMetadata_CreatedBy",
            schema: "Files",
            table: "FileMetadata",
            column: "CreatedBy");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameTable(
            schema: "Files",
            name: "FileMetadata");
    }
}
