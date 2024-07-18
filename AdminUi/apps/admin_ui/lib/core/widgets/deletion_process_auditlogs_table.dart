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
  final messageTemplates = MessageTemplate();

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
                DataCell(
                  Builder(
                    builder: (buildContext) => Text(
                      messageTemplates.getMessageForDeletionProcessAuditLog(buildContext, auditLog.messageKey, auditLog.additionalData),
                    ),
                  ),
                ),
                DataCell(auditLog.oldStatus == null ? const Text('') : Text(styleStatus(auditLog.oldStatus!))),
                DataCell(Text(styleStatus(auditLog.newStatus))),
              ],
            );
          }).toList(),
        ),
      ),
    );
  }

  String styleStatus(String status) {
    if (status == 'WaitingForApproval') return context.l10n.deletionProcessDetails_auditLogs_waitingForApproval;
    return status;
  }
}
