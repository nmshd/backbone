import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';

class IdentityDataTableSource extends DataTableSource {
  List<IdentityOverview> identities = [];
  Pagination? pagination;
  int sortColumnIndex = 0;
  bool sortAscending = true;

  void setData(
    List<IdentityOverview> identities,
    Pagination pagination,
    int columnIndex, {
    required bool columnAscending,
  }) {
    this.identities = identities;
    this.pagination = pagination;
    notifyListeners();

    sortColumnIndex = columnIndex;
    sortAscending = columnAscending;
    sort();
  }

  void sort() {
    identities.sort((a, b) {
      final aValue = _getDisplayValue(a);
      final bValue = _getDisplayValue(b);
      return sortAscending ? Comparable.compare(aValue, bValue) : Comparable.compare(bValue, aValue);
    });
    notifyListeners();
  }

  Comparable<Object> _getDisplayValue(IdentityOverview identity) {
    switch (sortColumnIndex) {
      case 0:
        return identity.address;
      case 1:
        return identity.tier.name;
      case 2:
        return identity.createdWithClient;
      case 3:
        return identity.numberOfDevices;
      case 4:
        return identity.createdAt;
      case 5:
        return identity.lastLoginAt.toString();
      case 6:
        return identity.datawalletVersion.toString();
      case 7:
        return identity.identityVersion;
      default:
        throw Exception('Invalid column index');
    }
  }

  @override
  DataRow2? getRow(int index) {
    for (var identity in identities) {
      return DataRow2(
        onLongPress: () {
          // Navigator.of(context).push(MaterialPageRoute(
          //   builder: (context) => IdentityDetails(identity: identity),
          // ));
        },
        cells: [
          DataCell(Text(identity.address)),
          DataCell(
            Center(
              child: GestureDetector(
                onTap: () {
                  // TODO(stamenione): Navigate to the tier details screen
                },
                child: Text(identity.tier.name),
              ),
            ),
          ),
          DataCell(Center(child: Text(identity.createdWithClient))),
          DataCell(Center(child: Text(identity.numberOfDevices.toString()))),
          DataCell(Center(child: Text(identity.createdAt.toString().substring(0, 10)))),
          DataCell(Center(child: Text(identity.lastLoginAt != null ? identity.lastLoginAt.toString() : ''))),
          DataCell(Center(child: Text(identity.datawalletVersion != null ? identity.datawalletVersion.toString() : ''))),
          DataCell(Center(child: Text(identity.identityVersion.toString()))),
        ],
      );
    }
  }

  @override
  bool get isRowCountApproximate => false;

  @override
  int get rowCount => pagination?.totalRecords ?? 0;

  @override
  int get selectedRowCount => 0;
}
