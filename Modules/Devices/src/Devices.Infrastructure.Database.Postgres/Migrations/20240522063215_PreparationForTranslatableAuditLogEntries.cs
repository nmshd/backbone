using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.Database.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class PreparationForTranslatableAuditLogEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename the column
            migrationBuilder.RenameColumn(
                name: "Message",
                schema: "Devices",
                table: "IdentityDeletionProcessAuditLog",
                newName: "MessageKey");

            // Perform the update with a validation step
            migrationBuilder.Sql(@"
                DO $$
                DECLARE
                    unmatched_count INTEGER;
                BEGIN
                    UPDATE ""Devices"".""IdentityDeletionProcessAuditLog""
                    SET ""MessageKey"" = CASE ""MessageKey""
                        WHEN 'The deletion process was started by the owner. It was automatically approved.' THEN 'StartedByOwner'
                        WHEN 'The deletion process was started by support. It is now waiting for approval.' THEN 'StartedBySupport'
                        WHEN 'The deletion process was approved.' THEN 'Approved'
                        WHEN 'The deletion process was rejected.' THEN 'Rejected'
                        WHEN 'The deletion process was cancelled by the owner of the identity.' THEN 'CancelledByOwner'
                        WHEN 'The deletion process was cancelled by a support employee.' THEN 'CancelledBySupport'
                        WHEN 'The deletion process was cancelled automatically, because it wasn''t approved by the owner within the approval period.' THEN 'CancelledAutomatically'
                        WHEN 'The first approval reminder notification has been sent.' THEN 'ApprovalReminder1Sent'
                        WHEN 'The second approval reminder notification has been sent.' THEN 'ApprovalReminder2Sent'
                        WHEN 'The third approval reminder notification has been sent.' THEN 'ApprovalReminder3Sent'
                        WHEN 'The first grace period reminder notification has been sent.' THEN 'GracePeriodReminder1Sent'
                        WHEN 'The second grace period reminder notification has been sent.' THEN 'GracePeriodReminder2Sent'
                        WHEN 'The third grace period reminder notification has been sent.' THEN 'GracePeriodReminder3Sent'
                        ELSE NULL
                    END;

                    SELECT COUNT(*)
                    INTO unmatched_count
                    FROM ""Devices"".""IdentityDeletionProcessAuditLog""
                    WHERE ""MessageKey"" IS NULL;

                    IF unmatched_count > 0 THEN
                        RAISE EXCEPTION 'There are invalid MessageKey values in the IdentityDeletionProcessAuditLog table.';
                    END IF;
                END
                $$;
            ");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MessageKey",
                schema: "Devices",
                table: "IdentityDeletionProcessAuditLog",
                newName: "Message");

            migrationBuilder.Sql(@"
                UPDATE ""Devices"".""IdentityDeletionProcessAuditLog""
                SET ""Message"" = CASE ""Message""
                    WHEN 'StartedByOwner' THEN 'The deletion process was started by the owner. It was automatically approved.'
                    WHEN 'StartedBySupport' THEN 'The deletion process was started by support. It is now waiting for approval.'
                    WHEN 'Approved' THEN 'The deletion process was approved.'
                    WHEN 'Rejected' THEN 'The deletion process was rejected.'
                    WHEN 'CancelledByOwner' THEN 'The deletion process was cancelled by the owner of the identity.'
                    WHEN 'CancelledBySupport' THEN 'The deletion process was cancelled by a support employee.'
                    WHEN 'CancelledAutomatically' THEN 'The deletion process was cancelled automatically, because it wasn''t approved by the owner within the approval period.'
                    WHEN 'ApprovalReminder1Sent' THEN 'The first approval reminder notification has been sent.'
                    WHEN 'ApprovalReminder2Sent' THEN 'The second approval reminder notification has been sent.'
                    WHEN 'ApprovalReminder3Sent' THEN 'The third approval reminder notification has been sent.'
                    WHEN 'GracePeriodReminder1Sent' THEN 'The first grace period reminder notification has been sent.'
                    WHEN 'GracePeriodReminder2Sent' THEN 'The second grace period reminder notification has been sent.'
                    WHEN 'GracePeriodReminder3Sent' THEN 'The third grace period reminder notification has been sent.'
                    ELSE ""Message""
                END;
            ");
        }
    }
}
