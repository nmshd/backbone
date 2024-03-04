using Microsoft.EntityFrameworkCore.Migrations;


namespace Backbone.Modules.Messages.Infrastructure.Database.SqlServer.Migrations;

public partial class Init : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Messages",
            columns: table => new
            {
                Id = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "char(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: false),
                CreatedByDevice = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                DoNotSendBefore = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Messages", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Attachments",
            columns: table => new
            {
                Id = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                MessageId = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Attachments", x => new { x.Id, x.MessageId });
                table.ForeignKey(
                    name: "FK_Attachments_Messages_MessageId",
                    column: x => x.MessageId,
                    principalTable: "Messages",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "RecipientInformation",
            columns: table => new
            {
                Address = table.Column<string>(type: "char(36)", unicode: false, fixedLength: true, maxLength: 36, nullable: false),
                MessageId = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                EncryptedKey = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                ReceivedByDevice = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: true),
                RelationshipId = table.Column<string>(type: "char(20)", unicode: false, fixedLength: true, maxLength: 20, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RecipientInformation", x => new { x.Address, x.MessageId });
                table.ForeignKey(
                    name: "FK_RecipientInformation_Messages_MessageId",
                    column: x => x.MessageId,
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
            });

        migrationBuilder.CreateIndex(
            name: "IX_Attachments_MessageId",
            table: "Attachments",
            column: "MessageId");

        migrationBuilder.CreateIndex(
            name: "IX_Messages_CreatedAt",
            table: "Messages",
            column: "CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_Messages_CreatedBy",
            table: "Messages",
            column: "CreatedBy");

        migrationBuilder.CreateIndex(
            name: "IX_Messages_DoNotSendBefore",
            table: "Messages",
            column: "DoNotSendBefore");

        migrationBuilder.CreateIndex(
            name: "IX_RecipientInformation_MessageId",
            table: "RecipientInformation",
            column: "MessageId");

        migrationBuilder.CreateIndex(
            name: "IX_RecipientInformation_ReceivedAt",
            table: "RecipientInformation",
            column: "ReceivedAt");

        migrationBuilder.CreateIndex(
            name: "IX_RecipientInformation_RelationshipId",
            table: "RecipientInformation",
            column: "RelationshipId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Attachments");

        migrationBuilder.DropTable(
            name: "RecipientInformation");

        migrationBuilder.DropTable(
            name: "Messages");
    }
}
