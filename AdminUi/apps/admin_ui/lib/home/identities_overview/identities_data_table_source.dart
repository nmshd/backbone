import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:logger/logger.dart';

class IdentityDataTableSource extends AsyncDataTableSource {
  List<IdentityOverview> data = [];
  Pagination? pagination;
  int sortColumnIndex = 0;
  bool sortAscending = true;

  IdentityOverviewFilter? filter;

  void setData(
    List<IdentityOverview> identities,
    Pagination pagination,
    int columnIndex, {
    required bool columnAscending,
  }) {
    data = identities;
    this.pagination = pagination;
    notifyListeners();

    sortColumnIndex = columnIndex;
    sortAscending = columnAscending;
    sort();
  }

  void sort() {
    data.sort((a, b) {
      final aValue = _getDisplayValue(a);
      final bValue = _getDisplayValue(b);
      return sortAscending ? Comparable.compare(aValue, bValue) : Comparable.compare(bValue, aValue);
    });
    notifyListeners();
  }

  Comparable<Object> _getDisplayValue(IdentityOverview identity) {
    return switch (sortColumnIndex) {
      0 => identity.address as Comparable<Object>,
      1 => identity.tier.name as Comparable<Object>,
      2 => identity.createdWithClient as Comparable<Object>,
      3 => identity.numberOfDevices as Comparable<Object>,
      4 => identity.createdAt as Comparable<Object>,
      5 => identity.lastLoginAt.toString() as Comparable<Object>,
      6 => identity.datawalletVersion.toString() as Comparable<Object>,
      7 => identity.identityVersion as Comparable<Object>,
      _ => throw Exception('Invalid column index')
    };
  }

  @override
  bool get isRowCountApproximate => false;

  @override
  int get rowCount => pagination?.totalRecords ?? 0;

  @override
  int get selectedRowCount => 0;

  @override
  Future<AsyncRowsResponse> getRows(int startIndex, int count) async {
    final pageNumber = (startIndex ~/ count) + 1;

    try {
      final response = await GetIt.I.get<AdminApiClient>().identities.getIdentities(pageNumber: pageNumber, pageSize: count, filter: filter);
      pagination = response.pagination;

      final rows = response.data.indexed
          .map(
            (identity) => DataRow.byIndex(
              index: pageNumber * count + identity.$1,
              cells: [
                DataCell(Text(identity.$2.address)),
                DataCell(Text(identity.$2.tier.name)),
                DataCell(Text(identity.$2.createdWithClient)),
                DataCell(Text(identity.$2.numberOfDevices.toString())),
                DataCell(Text(identity.$2.createdAt.toString().substring(0, 10))),
                DataCell(Text(identity.$2.lastLoginAt?.toString().substring(0, 10) ?? '')),
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
}
