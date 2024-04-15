using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quotas.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class MetricStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExhaustedUntil",
                schema: "Quotas",
                table: "TierQuotas");

            migrationBuilder.CreateTable(
                name: "MetricStatus",
                schema: "Quotas",
                columns: table => new
                {
                    MetricKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Owner = table.Column<string>(type: "character(36)", nullable: false),
                    IsExhaustedUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetricStatus", x => new { x.Owner, x.MetricKey });
                    table.ForeignKey(
                        name: "FK_MetricStatus_Identities_Owner",
                        column: x => x.Owner,
                        principalSchema: "Quotas",
                        principalTable: "Identities",
                        principalColumn: "Address",
                        onDelete: ReferentialAction.Cascade);
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MetricStatus");

            migrationBuilder.AddColumn<DateTime>(
                name: "IsExhaustedUntil",
                schema: "Quotas",
                table: "TierQuotas",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
