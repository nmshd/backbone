﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Messages.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class IdForRecipientInformation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RecipientInformation",
                table: "RecipientInformation");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "RecipientInformation",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecipientInformation",
                table: "RecipientInformation",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RecipientInformation_Address_MessageId",
                table: "RecipientInformation",
                columns: new[] { "Address", "MessageId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RecipientInformation",
                table: "RecipientInformation");

            migrationBuilder.DropIndex(
                name: "IX_RecipientInformation_Address_MessageId",
                table: "RecipientInformation");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "RecipientInformation");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecipientInformation",
                table: "RecipientInformation",
                columns: new[] { "Address", "MessageId" });
        }
    }
}
