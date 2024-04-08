using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quotas.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class MetricStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExhaustedUntil",
                table: "TierQuotas");

            migrationBuilder.CreateTable(
                name: "MetricStatus",
                schema: "Quotas",
                columns: table => new
                {
                    MetricKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Owner = table.Column<string>(type: "char(36)", nullable: false),
                    IsExhaustedUntil = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetricStatus", x => new { x.Owner, x.MetricKey });
                    table.ForeignKey(
                        name: "FK_MetricStatus_Identities_Owner",
                        column: x => x.Owner,
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
                table: "TierQuotas",
                type: "datetime2",
                nullable: true);
        }
    }
}
