import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';

class ClientDataTableSource extends DataTableSource {
  final List<Clients> clients;
  final List<bool> rowCheckboxState;
  final void Function(Clients) onRowTap;
  bool sortAscending;
  int sortColumnIndex;

  ClientDataTableSource(
    this.clients,
    this.rowCheckboxState,
    this.onRowTap,
    this.sortColumnIndex, {
    required this.sortAscending,
  });

  @override
  DataRow getRow(int index) {
    final client = clients[index];
    return DataRow(
      cells: [
        DataCell(Text(client.clientId)),
        DataCell(Text(client.displayName)),
        DataCell(Text(client.defaultTier.name)),
        DataCell(Text(client.numberOfIdentities.toString())),
        DataCell(Text(client.createdAt)),
        DataCell(
          Row(
            children: [
              ElevatedButton(
                onPressed: () => onRowTap(client),
                child: const Text('Change Client Secret'),
              ),
            ],
          ),
        ),
      ],
      onSelectChanged: (selected) {},
    );
  }

  @override
  bool get isRowCountApproximate => false;

  @override
  int get rowCount => clients.length;

  @override
  int get selectedRowCount => 0;

  void sort<T>(Comparable<T> Function(Clients client) getField, int columnIndex, {bool ascending = true}) {
    clients.sort((a, b) {
      final aValue = getField(a);
      final bValue = getField(b);
      return ascending ? Comparable.compare(aValue, bValue) : Comparable.compare(bValue, aValue);
    });
    notifyListeners();
  }

  void onSort(int columnIndex, {bool ascending = false}) {
    switch (columnIndex) {
      case 0:
        sort((client) => client.clientId, columnIndex, ascending: ascending);
      case 1:
        sort((client) => client.displayName, columnIndex, ascending: ascending);
    }
    sortColumnIndex = columnIndex;
    sortAscending = ascending;
  }
}
