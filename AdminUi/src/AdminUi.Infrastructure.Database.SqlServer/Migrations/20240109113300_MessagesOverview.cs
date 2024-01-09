﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.AdminUi.Infrastructure.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class MessagesOverview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE VIEW MessageOverviews AS
                    SELECT
        	            [MESSAGES].[Id] AS [MessageId],
        	            [MESSAGES].[CreatedBy] AS [SenderAddress],
        	            [MESSAGES].[CreatedByDevice] AS [SenderDevice],
        	            [MESSAGES].[CreatedAt] AS [SendDate],
        	            COUNT ([Attachments].[Id]) AS [NumberOfAttachments]
                    FROM
        	            [Messages].[Messages] AS MESSAGES
                    LEFT JOIN
        	            [Messages].[Attachments] AS ATTACHMENTS
                    ON
        	            [MESSAGES].[Id] = [ATTACHMENTS].[MessageId]
                    GROUP BY
        	            [MESSAGES].[Id], [MESSAGES].[CreatedBy], [MESSAGES].[CreatedByDevice], [MESSAGES].[CreatedAt]
        """);

            migrationBuilder.Sql("""
		        CREATE VIEW MessageRecipients AS
		            SELECT
			            [RECIPIENTS].[Address] AS [Address],
			            [RECIPIENTS].[MessageId] AS [MessageId]
		            FROM
			            [Messages].[RecipientInformation] AS RECIPIENTS
		""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(""" DROP VIEW "MessageOverviews" """);
            migrationBuilder.Sql(""" DROP VIEW "MessageRecipients" """);
        }
    }
}
