using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Messages.Infrastructure.Database.Postgres.Migrations;

/// <inheritdoc />
public partial class Init : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Messages",
            schema: "Messages",
            columns: table => new
            {
                Id = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "character(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: false),
                CreatedByDevice = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                DoNotSendBefore = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Messages", x => x.Id);
            }
        );

        migrationBuilder.CreateTable(
            name: "Attachments",
            schema: "Messages",
            columns: table => new
            {
                Id = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                MessageId = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Attachments", x => new { x.Id, x.MessageId });
                table.ForeignKey(
                    name: "FK_Attachments_Messages_MessageId",
                    column: x => x.MessageId,
                    principalSchema: "Messages",
                    principalTable: "Messages",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            }
        );

        migrationBuilder.CreateTable(
            name: "RecipientInformation",
            schema: "Messages",
            columns: table => new
            {
                Address = table.Column<string>(type: "character(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: false),
                MessageId = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                EncryptedKey = table.Column<byte[]>(type: "bytea", nullable: false),
                ReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                ReceivedByDevice = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true),
                RelationshipId = table.Column<string>(type: "character(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RecipientInformation", x => new { x.Address, x.MessageId });
                table.ForeignKey(
                    name: "FK_RecipientInformation_Messages_MessageId",
                    column: x => x.MessageId,
                    principalSchema: "Messages",
                    principalTable: "Messages",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_RecipientInformation_Relationships_RelationshipId",
                    column: x => x.RelationshipId,
                    principalSchema: "Relationships",
                    principalTable: "Relationships",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            }
        );

        migrationBuilder.CreateIndex(
            name: "IX_Attachments_MessageId",
            schema: "Messages",
            table: "Attachments",
            column: "MessageId");

        migrationBuilder.CreateIndex(
            name: "IX_Messages_CreatedAt",
            schema: "Messages",
            table: "Messages",
            column: "CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_Messages_CreatedBy",
            schema: "Messages",
            table: "Messages",
            column: "CreatedBy");

        migrationBuilder.CreateIndex(
            name: "IX_Messages_DoNotSendBefore",
            schema: "Messages",
            table: "Messages",
            column: "DoNotSendBefore");

        migrationBuilder.CreateIndex(
            name: "IX_RecipientInformation_MessageId",
            schema: "Messages",
            table: "RecipientInformation",
            column: "MessageId");

        migrationBuilder.CreateIndex(
            name: "IX_RecipientInformation_ReceivedAt",
            schema: "Messages",
            table: "RecipientInformation",
            column: "ReceivedAt");

        migrationBuilder.CreateIndex(
            name: "IX_RecipientInformation_RelationshipId",
            schema: "Messages",
            table: "RecipientInformation",
            column: "RelationshipId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            schema: "Messages",
            name: "Attachments");

        migrationBuilder.DropTable(
            schema: "Messages",
            name: "RecipientInformation");

        migrationBuilder.DropTable(
            schema: "Messages",
            name: "Messages");
    }
}
