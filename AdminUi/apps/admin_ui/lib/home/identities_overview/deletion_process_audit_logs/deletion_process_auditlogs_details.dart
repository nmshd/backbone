import 'dart:io';

import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:intl/intl.dart';

import '/core/core.dart';

class DeletionProcessAuditLogsDetails extends StatefulWidget {
  final String identityAddress;
  final Locale locale;

  const DeletionProcessAuditLogsDetails({
    required this.identityAddress,
    required this.locale,
    super.key,
  });

  @override
  State<DeletionProcessAuditLogsDetails> createState() => _DeletionProcessAuditLogsDetailsState();
}

class _DeletionProcessAuditLogsDetailsState extends State<DeletionProcessAuditLogsDetails> {
  List<AuditLogs>? _identityDeletionProcessAuditLogs;

  @override
  void initState() {
    super.initState();
    _loadIdentityDeletionProcessAuditLogs();
  }

  @override
  Widget build(BuildContext context) {
    if (_identityDeletionProcessAuditLogs == null) {
      return const Center(child: CircularProgressIndicator());
    }

    final identityDeletionProcessAuditLogs = _identityDeletionProcessAuditLogs!;
    return Column(
      mainAxisSize: MainAxisSize.min,
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        if (Platform.isMacOS || Platform.isWindows) const BackButton(),
        Row(
          children: [
            Expanded(
              child: Card(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Padding(
                      padding: const EdgeInsets.all(16),
                      child: Text(
                        context.l10n.deletionProcessAuditLogsDetails_title,
                        style: const TextStyle(fontSize: 24, fontWeight: FontWeight.w600),
                      ),
                    ),
                    Padding(
                      padding: const EdgeInsets.all(16),
                      child: Text(
                        '${context.l10n.deletionProcessAuditLogsDetails_title_description}:  ${widget.identityAddress}',
                        style: const TextStyle(fontSize: 16, fontWeight: FontWeight.w600),
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
        Expanded(
          child: Card(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: DataTable2(
                empty: Text(context.l10n.deletionProcessAuditLogsDetails_noDataFound),
                columns: const <DataColumn2>[
                  DataColumn2(label: Text('ID'), size: ColumnSize.S),
                  DataColumn2(label: Text('Created At'), size: ColumnSize.S),
                  DataColumn2(label: Text('Message'), size: ColumnSize.L),
                  DataColumn2(label: Text('Old Status'), size: ColumnSize.S),
                  DataColumn2(label: Text('New Status'), size: ColumnSize.S),
                ],
                rows: identityDeletionProcessAuditLogs.map((auditLog) {
                  return DataRow(
                    cells: [
                      DataCell(Text(auditLog.id)),
                      DataCell(
                        Text(
                          '${DateFormat.yMd(widget.locale.languageCode).format(auditLog.createdAt)} ${DateFormat.Hms().format(auditLog.createdAt)}',
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
          ),
        ),
      ],
    );
  }

  String replaceMessageKeyWithCorrespondingText(String messageKey) {
    switch (messageKey) {
      case 'StartedByOwner':
        return context.l10n.deletionProcessAuditLogsDetails_startedByOwner;
      case 'StartedBySupport':
        return context.l10n.deletionProcessAuditLogsDetails_startedBySupport;
      case 'Approved':
        return context.l10n.deletionProcessAuditLogsDetails_approved;
      case 'Rejected':
        return context.l10n.deletionProcessAuditLogsDetails_rejected;
      case 'CancelledByOwner':
        return context.l10n.deletionProcessAuditLogsDetails_cancelledByOwner;
      case 'CancelledBySupport':
        return context.l10n.deletionProcessAuditLogsDetails_cancelledBySupport;
      case 'CancelledAutomatically':
        return context.l10n.deletionProcessAuditLogsDetails_cancelledAutomatically;
      case 'ApprovalReminder1Sent':
        return context.l10n.deletionProcessAuditLogsDetails_approvalReminder1Sent;
      case 'ApprovalReminder2Sent':
        return context.l10n.deletionProcessAuditLogsDetails_approvalReminder2Sent;
      case 'ApprovalReminder3Sent':
        return context.l10n.deletionProcessAuditLogsDetails_approvalReminder3Sent;
      case 'GracePeriodReminder1Sent':
        return context.l10n.deletionProcessAuditLogsDetails_gracePeriodReminder1Sent;
      case 'GracePeriodReminder2Sent':
        return context.l10n.deletionProcessAuditLogsDetails_gracePeriodReminder2Sent;
      case 'GracePeriodReminder3Sent':
        return context.l10n.deletionProcessAuditLogsDetails_gracePeriodReminder3Sent;
      default:
        return context.l10n.deletionProcessAuditLogsDetails_unknownMessageKey;
    }
  }

  String styleStatus(String status) {
    if (status == 'WaitingForApproval') return context.l10n.deletionProcessAuditLogsDetails_waitingForApproval;
    return status;
  }

  Future<void> _loadIdentityDeletionProcessAuditLogs() async {
    final response = await GetIt.I.get<AdminApiClient>().identities.getIdentityAuditLogs(address: widget.identityAddress);
    if (mounted) {
      setState(() {
        _identityDeletionProcessAuditLogs = response.data;
      });
    }
  }
}
