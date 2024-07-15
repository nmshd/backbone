import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

import '/core/core.dart';

class DeletionProcessAuditLogsTable extends StatefulWidget {
  final List<AuditLog> auditLogs;

  const DeletionProcessAuditLogsTable({
    required this.auditLogs,
    super.key,
  });

  @override
  State<DeletionProcessAuditLogsTable> createState() => _DeletionProcessAuditLogsTableState();
}

class _DeletionProcessAuditLogsTableState extends State<DeletionProcessAuditLogsTable> {
  @override
  Widget build(BuildContext context) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: DataTable2(
          empty: Text(context.l10n.deletionProcessDetails_auditLogs_noData),
          columns: const <DataColumn2>[
            DataColumn2(label: Text('ID'), size: ColumnSize.S),
            DataColumn2(label: Text('Created At'), size: ColumnSize.S),
            DataColumn2(label: Text('Message'), size: ColumnSize.L),
            DataColumn2(label: Text('Old Status'), size: ColumnSize.S),
            DataColumn2(label: Text('New Status'), size: ColumnSize.S),
          ],
          rows: widget.auditLogs.map((auditLog) {
            return DataRow(
              cells: [
                DataCell(Text(auditLog.id)),
                DataCell(
                  Text(
                    '${DateFormat.yMd(Localizations.localeOf(context).languageCode).format(auditLog.createdAt)} ${DateFormat.Hms().format(auditLog.createdAt)}',
                  ),
                ),
                DataCell(Text(replaceMessageKeyWithCorrespondingText(auditLog.messageKey))),
                DataCell(auditLog.oldStatus == null ? const Text('') : Text(styleStatus(auditLog.oldStatus!))),
                DataCell(Text(styleStatus(auditLog.newStatus))),
              ],
            );
          }).toList(),
        ),
      ),
    );
  }

  String replaceMessageKeyWithCorrespondingText(String messageKey) {
    switch (messageKey) {
      case 'StartedByOwner':
        return context.l10n.deletionProcessDetails_auditLogs_startedByOwner;
      case 'StartedBySupport':
        return context.l10n.deletionProcessDetails_auditLogs_startedBySupport;
      case 'Approved':
        return context.l10n.deletionProcessDetails_auditLogs_approved;
      case 'Rejected':
        return context.l10n.deletionProcessDetails_auditLogs_rejected;
      case 'CancelledByOwner':
        return context.l10n.deletionProcessDetails_auditLogs_cancelledByOwner;
      case 'CancelledBySupport':
        return context.l10n.deletionProcessDetails_auditLogs_cancelledBySupport;
      case 'CancelledAutomatically':
        return context.l10n.deletionProcessDetails_auditLogs_cancelledAutomatically;
      case 'ApprovalReminder1Sent':
        return context.l10n.deletionProcessDetails_auditLogs_approvalReminder1Sent;
      case 'ApprovalReminder2Sent':
        return context.l10n.deletionProcessDetails_auditLogs_approvalReminder2Sent;
      case 'ApprovalReminder3Sent':
        return context.l10n.deletionProcessDetails_auditLogs_approvalReminder3Sent;
      case 'GracePeriodReminder1Sent':
        return context.l10n.deletionProcessDetails_auditLogs_gracePeriodReminder1Sent;
      case 'GracePeriodReminder2Sent':
        return context.l10n.deletionProcessDetails_auditLogs_gracePeriodReminder2Sent;
      case 'GracePeriodReminder3Sent':
        return context.l10n.deletionProcessDetails_auditLogs_gracePeriodReminder3Sent;
      default:
        return context.l10n.deletionProcessDetails_auditLogs_unknownMessageKey;
    }
  }

  String styleStatus(String status) {
    if (status == 'WaitingForApproval') return context.l10n.deletionProcessDetails_auditLogs_waitingForApproval;
    return status;
  }
}
