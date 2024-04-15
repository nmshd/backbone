﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Messages.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDoNotSendBefore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_DoNotSendBefore",
                schema: "Messages",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "DoNotSendBefore",
                schema: "Messages",
                table: "Messages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DoNotSendBefore",
                schema: "Messages",
                table: "Messages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_DoNotSendBefore",
                schema: "Messages",
                table: "Messages",
                column: "DoNotSendBefore");
        }
    }
}
