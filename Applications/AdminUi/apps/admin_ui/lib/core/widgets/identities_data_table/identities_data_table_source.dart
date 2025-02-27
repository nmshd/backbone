import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:intl/intl.dart';
import 'package:logger/logger.dart';

class IdentityDataTableSource extends AsyncDataTableSource {
  Pagination? _pagination;
  var _sortingSettings = (sortColumnIndex: 0, sortAscending: true);
  IdentityOverviewFilter? _filter;
  set filter(IdentityOverviewFilter? newFilter) {
    if (_filter != newFilter) {
      _filter = newFilter;
      notifyListeners();
    }
  }

  final Locale locale;
  final bool hideTierColumn;
  final bool hideClientColumn;
  final void Function({required String address}) navigateToIdentity;

  IdentityDataTableSource({
    required this.locale,
    required this.navigateToIdentity,
    this.hideTierColumn = false,
    this.hideClientColumn = false,
    IdentityOverviewFilter? filter,
  }) : _filter = filter;
  void sort({required int sortColumnIndex, required bool sortColumnAscending}) {
    _sortingSettings = (sortColumnIndex: sortColumnIndex, sortAscending: sortColumnAscending);
    notifyListeners();
  }

  @override
  bool get isRowCountApproximate => false;

  @override
  int get rowCount => _pagination?.totalRecords ?? 0;

  @override
  int get selectedRowCount => 0;

  @override
  Future<AsyncRowsResponse> getRows(int startIndex, int count) async {
    final pageNumber = startIndex ~/ count;
    final orderBy = _getODataOrderBy();
    try {
      final response = await GetIt.I.get<AdminApiClient>().identities.getIdentities(
        pageNumber: pageNumber,
        pageSize: count,
        filter: _filter,
        orderBy: orderBy,
      );
      _pagination = response.pagination;

      final rows =
          response.data.indexed
              .map(
                (identity) => DataRow2.byIndex(
                  index: pageNumber * count + identity.$1,
                  onTap: () => navigateToIdentity(address: identity.$2.address),
                  cells: [
                    DataCell(Text(identity.$2.address)),
                    if (!hideTierColumn) DataCell(Text(identity.$2.tier.name)),
                    if (!hideClientColumn) DataCell(Text(identity.$2.createdWithClient)),
                    DataCell(Text(identity.$2.numberOfDevices.toString())),
                    DataCell(
                      Tooltip(
                        message:
                            '${DateFormat.yMd(locale.languageCode).format(identity.$2.createdAt)} ${DateFormat.Hms().format(identity.$2.createdAt)}',
                        child: Text(DateFormat.yMd(locale.languageCode).format(identity.$2.createdAt)),
                      ),
                    ),
                    DataCell(
                      Tooltip(
                        message:
                            identity.$2.lastLoginAt != null
                                ? '${DateFormat.yMd(locale.languageCode).format(identity.$2.lastLoginAt!)} ${DateFormat.Hms().format(identity.$2.lastLoginAt!)}'
                                : '',
                        child: Text(identity.$2.lastLoginAt != null ? DateFormat.yMd(locale.languageCode).format(identity.$2.lastLoginAt!) : ''),
                      ),
                    ),
                    DataCell(Text(identity.$2.datawalletVersion?.toString() ?? '')),
                    DataCell(Text(identity.$2.identityVersion.toString())),
                  ],
                ),
              )
              .toList();

      return AsyncRowsResponse(response.pagination.totalRecords, rows);
    } catch (e) {
      GetIt.I.get<Logger>().e('Failed to load data: $e');
      throw Exception('Failed to load data: $e');
    }
  }

  String _getODataOrderBy() {
    final columnName = _getFieldNameByIndex(_sortingSettings.sortColumnIndex);
    final sortingDirection = _sortingSettings.sortAscending ? 'asc' : 'desc';
    return '$columnName $sortingDirection';
  }

  String _getFieldNameByIndex(int index) => switch (index) {
    0 => 'address',
    2 => 'createdWithClient',
    3 => 'numberOfDevices',
    4 => 'createdAt',
    5 => 'lastLoginAt',
    6 => 'datawalletVersion',
    7 => 'identityVersion',
    _ => throw Exception('Invalid column index'),
  };
}
