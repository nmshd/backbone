using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.AdminUi.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class MessagesOverview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE VIEW "MessageOverviews" AS
                    SELECT
        	            "Messages"."Id" AS "MessageId",
        	            "Messages"."CreatedBy" AS "SenderAddress",
        	            "Messages"."CreatedByDevice" AS "SenderDevice",
        	            "Messages"."CreatedAt" AS "SendDate",
        	            COUNT ("Attachments"."Id") AS "NumberOfAttachments"
                    FROM
        	            "Messages"."Messages" AS "Messages"
                    LEFT JOIN
        	            "Messages"."Attachments" AS "Attachments"
                    ON
        	            "Messages"."Id" = "Attachments"."MessageId"
                    GROUP BY
        	            "Messages"."Id", "Messages"."CreatedBy", "Messages"."CreatedByDevice", "Messages"."CreatedAt"
        """);

            migrationBuilder.Sql("""
					CREATE VIEW "MessageRecipients" AS
						SELECT
							"Recipients"."Address" AS "Address",
							"Recipients"."MessageId" AS "MessageId"
						FROM
							"Messages"."RecipientInformation" AS "Recipients"
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
