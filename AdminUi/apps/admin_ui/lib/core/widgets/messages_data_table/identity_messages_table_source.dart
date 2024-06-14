import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:intl/intl.dart';
import 'package:logger/logger.dart';

class IdentityMessagesTableSource extends AsyncDataTableSource {
  Pagination? _pagination;

  final Locale locale;
  final bool hideTierColumn;
  final String address;
  final String type;

  IdentityMessagesTableSource({
    required this.address,
    required this.type,
    required this.locale,
    this.hideTierColumn = false,
  });

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
      final response = await GetIt.I.get<AdminApiClient>().messages.getMessagesByParticipantAddress(
            address: address,
            type: type,
            pageNumber: pageNumber,
            pageSize: count,
          );

      _pagination = response.isPaged
          ? response.pagination
          : Pagination(
              pageNumber: pageNumber,
              pageSize: count,
              totalPages: _totalPages(count, response.data),
              totalRecords: response.data.length,
            );

      final rows = response.data.indexed
          .map(
            (message) => DataRow2.byIndex(
              index: pageNumber * count + message.$1,
              cells: [
                DataCell(Text(message.$2.recipients.map((recipient) => recipient.address).join(', '))),
                DataCell(Text(message.$2.senderAddress)),
                DataCell(Text(message.$2.senderDevice)),
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
