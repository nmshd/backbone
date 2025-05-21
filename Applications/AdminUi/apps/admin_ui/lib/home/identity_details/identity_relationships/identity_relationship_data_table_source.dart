import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:intl/intl.dart';
import 'package:logger/logger.dart';

class IdentityRelationshipDataTableSource extends AsyncDataTableSource {
  Pagination? _pagination;

  final Locale locale;
  final String address;

  IdentityRelationshipDataTableSource({required this.address, required this.locale});

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
      final response = await GetIt.I.get<AdminApiClient>().relationships.getRelationshipsByParticipantAddress(
        address,
        pageNumber: pageNumber,
        pageSize: count,
      );

      _pagination = response.isPaged
          ? response.pagination
          : Pagination(pageNumber: pageNumber, pageSize: count, totalPages: _totalPages(count, response.data), totalRecords: response.data.length);

      final rows = response.data.indexed
          .map(
            (relationship) => DataRow2.byIndex(
              index: pageNumber * count + relationship.$1,
              cells: [
                DataCell(Text(relationship.$2.peer)),
                DataCell(Text(relationship.$2.requestedBy)),
                DataCell(Text(relationship.$2.templateId)),
                DataCell(Text(relationship.$2.status)),
                DataCell(
                  Tooltip(
                    message:
                        '${DateFormat.yMd(locale.languageCode).format(relationship.$2.creationDate)} ${DateFormat.Hms().format(relationship.$2.creationDate)}',
                    child: Text(DateFormat.yMd(locale.languageCode).format(relationship.$2.creationDate)),
                  ),
                ),
                DataCell(
                  relationship.$2.answeredAt == null
                      ? const Text('-')
                      : Tooltip(
                          message:
                              '${DateFormat.yMd(locale.languageCode).format(relationship.$2.answeredAt!)} ${DateFormat.Hms().format(relationship.$2.answeredAt!)}',
                          child: Text(DateFormat.yMd(locale.languageCode).format(relationship.$2.answeredAt!)),
                        ),
                ),
                DataCell(Text(relationship.$2.createdByDevice)),
                DataCell(Text(relationship.$2.answeredByDevice ?? '-')),
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

  int _totalPages(int count, List<Relationship>? data) {
    if (data == null || data.isEmpty) return 1;
    return (data.length / count).ceil();
  }
}
