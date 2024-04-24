import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';

class IdentityDataTableSource extends AsyncDataTableSource {
  List<IdentityOverview> data = [];
  Pagination? pagination;
  int sortColumnIndex = 0;
  bool sortAscending = true;

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
  DataRow? getRow(int index) {
    final localIndex = index % data.length;

    if (localIndex < data.length) {
      final identity = data[localIndex];
      return DataRow.byIndex(
        index: localIndex,
        onLongPress: () {},
        cells: [
          DataCell(Text(identity.address)),
          DataCell(
            Center(
              child: GestureDetector(
                onTap: () {},
                child: Text(identity.tier.name),
              ),
            ),
          ),
          DataCell(Center(child: Text(identity.createdWithClient))),
          DataCell(Center(child: Text(identity.numberOfDevices.toString()))),
          DataCell(Center(child: Text(identity.createdAt.toString().substring(0, 10)))),
          DataCell(Center(child: Text(identity.lastLoginAt != null ? identity.lastLoginAt.toString().substring(0, 10) : ''))),
          DataCell(Center(child: Text(identity.datawalletVersion != null ? identity.datawalletVersion.toString() : ''))),
          DataCell(Center(child: Text(identity.identityVersion.toString()))),
        ],
      );
    }
    return null;
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
      final response = await GetIt.I.get<AdminApiClient>().identities.getIdentities(
            pageNumber: pageNumber,
            pageSize: count,
          );

      final rows = response.data
          .map(
            (identity) => DataRow(
              cells: [
                DataCell(Text(identity.address)),
                DataCell(Text(identity.tier.name)),
                DataCell(Text(identity.createdWithClient)),
                DataCell(Text(identity.numberOfDevices.toString())),
                DataCell(Text(identity.createdAt.toString().substring(0, 10))),
                DataCell(Text(identity.lastLoginAt?.toString().substring(0, 10) ?? '')),
                DataCell(Text(identity.datawalletVersion?.toString() ?? '')),
                DataCell(Text(identity.identityVersion.toString())),
              ],
            ),
          )
          .toList();

      return AsyncRowsResponse(response.pagination.totalRecords, rows);
    } catch (e) {
      throw Exception('Failed to load data: $e');
    }
  }
}
