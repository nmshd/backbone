import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

import '/core/core.dart';

class DeletionProcessAuditLogsTable extends StatefulWidget {
  final List<IdentityDeletionProcessAuditLogEntry> auditLogs;

  const DeletionProcessAuditLogsTable({required this.auditLogs, super.key});

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
          empty: Text(context.l10n.deletionProcessAuditLogsDetails_noDataFound),
          columns: const <DataColumn2>[
            DataColumn2(label: Text('ID'), size: .S),
            DataColumn2(label: Text('Created At'), size: .S),
            DataColumn2(label: Text('Message'), size: .L),
            DataColumn2(label: Text('Old Status'), size: .S),
            DataColumn2(label: Text('New Status'), size: .S),
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
                DataCell(Text(_getMessageForDeletionProcessAuditLog(auditLog.messageKey, auditLog.additionalData))),
                DataCell(auditLog.oldStatus == null ? const Text('') : Text(auditLog.oldStatus!)),
                DataCell(Text(auditLog.newStatus)),
              ],
            );
          }).toList(),
        ),
      ),
    );
  }

  String _getMessageForDeletionProcessAuditLog(String messageKey, Map<String, String> additionalData) {
    final messageTemplates = {
      'StartedByOwner': context.l10n.deletionProcessAuditLogsTable_startedByOwner,
      'CancelledByOwner': context.l10n.deletionProcessAuditLogsTable_cancelledByOwner,
      'GracePeriodReminder1Sent': context.l10n.deletionProcessAuditLogsTable_gracePeriodReminder1Sent,
      'GracePeriodReminder2Sent': context.l10n.deletionProcessAuditLogsTable_gracePeriodReminder2Sent,
      'GracePeriodReminder3Sent': context.l10n.deletionProcessAuditLogsTable_gracePeriodReminder3Sent,
      'DataDeleted': '${context.l10n.all} {aggregateType} ${context.l10n.deletionProcessAuditLogsTable_haveBeenDeleted}',
    };

    var messageTemplate = messageTemplates[messageKey];

    additionalData.forEach((key, value) {
      final placeholder = '{$key}';
      messageTemplate = messageTemplate?.replaceAll(placeholder, value);
    });

    return messageTemplate!;
  }
}
