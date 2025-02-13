import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';
import 'package:logger/logger.dart';

import '/core/core.dart';
import 'modals/all_recipients_dialog.dart';

class IdentityMessagesDataTableSource extends AsyncDataTableSource {
  Pagination? _pagination;

  final Locale locale;
  final String participant;
  final MessageType type;

  IdentityMessagesDataTableSource({required this.participant, required this.type, required this.locale});

  @override
  bool get isRowCountApproximate => false;

  @override
  int get rowCount => _pagination?.totalRecords ?? 0;

  @override
  int get selectedRowCount => 0;

  @override
  Future<AsyncRowsResponse> getRows(int startIndex, int count) async {
    final pageNumber = startIndex ~/ count + 1;
    try {
      final response = await GetIt.I.get<AdminApiClient>().messages.getMessagesByParticipant(
        participant: participant,
        type: type,
        pageNumber: pageNumber,
        pageSize: count,
      );

      _pagination =
          response.isPaged
              ? response.pagination
              : Pagination(
                pageNumber: pageNumber,
                pageSize: count,
                totalPages: _totalPages(count, response.data),
                totalRecords: response.data.length,
              );

      final rows =
          response.data.indexed
              .map(
                (message) => DataRow2.byIndex(
                  index: pageNumber * count + message.$1,
                  specificRowHeight:
                      message.$2.recipients.length > 3 && type == MessageType.outgoing
                          ? 100.0
                          : message.$2.recipients.length == 3 && type == MessageType.outgoing
                          ? 65.0
                          : null,
                  cells: [
                    if (type == MessageType.outgoing) DataCell(_RecipientsCell(recipients: message.$2.recipients)),
                    if (type == MessageType.incoming) ...[DataCell(Text(message.$2.senderAddress)), DataCell(Text(message.$2.senderDevice))],
                    DataCell(Text(message.$2.numberOfAttachments.toString())),
                    DataCell(
                      Tooltip(
                        message: '${DateFormat.yMd(locale.languageCode).format(message.$2.sendDate)} ${DateFormat.Hms().format(message.$2.sendDate)}',
                        child: Text(DateFormat.yMd(locale.languageCode).format(message.$2.sendDate)),
                      ),
                    ),
                  ],
                ),
              )
              .toList();
      return AsyncRowsResponse(response.isPaged ? response.pagination.totalPages : _pagination!.totalPages, rows);
    } catch (e) {
      GetIt.I.get<Logger>().e('Failed to load data: $e');
      throw Exception('Failed to load data: $e');
    }
  }

  int _totalPages(int count, List<MessageOverview>? data) {
    if (data == null || data.isEmpty) return 1;
    return (data.length / count).ceil();
  }
}

class _RecipientsCell extends StatelessWidget {
  final List<MessageRecipient> recipients;

  const _RecipientsCell({required this.recipients});

  @override
  Widget build(BuildContext context) {
    final displayedRecipients = recipients.length > 3 ? recipients.sublist(0, 3) : recipients;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        ...displayedRecipients.map(
          (recipient) => InkWell(onTap: () => context.push('/identities/${recipient.address}'), child: Text(recipient.address)),
        ),
        if (recipients.length > 3)
          FilledButton(onPressed: () => showAllRecipientsDialog(context: context, recipients: recipients), child: Text(context.l10n.showAll)),
      ],
    );
  }
}
