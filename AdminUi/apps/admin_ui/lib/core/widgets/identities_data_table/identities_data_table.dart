import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';

import '../../constants.dart';
import 'identities_data_table_source.dart';

export 'identities_data_table_source.dart';
export 'identities_filter.dart';

class IdentitiesDataTable extends StatefulWidget {
  final IdentityDataTableSource dataSource;
  final bool hideTierColumn;

  const IdentitiesDataTable({required this.dataSource, this.hideTierColumn = false, super.key});

  @override
  State<IdentitiesDataTable> createState() => _IdentitiesDataTableState();
}

class _IdentitiesDataTableState extends State<IdentitiesDataTable> {
  int _sortColumnIndex = 0;
  bool _sortColumnAscending = true;

  int _rowsPerPage = 5;

  @override
  Widget build(BuildContext context) {
    return AsyncPaginatedDataTable2(
      rowsPerPage: _rowsPerPage,
      onRowsPerPageChanged: _setRowsPerPage,
      sortColumnIndex: _sortColumnIndex,
      sortAscending: _sortColumnAscending,
      showFirstLastButtons: true,
      columnSpacing: 5,
      source: widget.dataSource,
      isVerticalScrollBarVisible: true,
      renderEmptyRowsInTheEnd: false,
      availableRowsPerPage: const [5, 10, 25, 50, 100],
      empty: const Text('No identities found.'),
      errorBuilder: (error) => Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Text('An error occurred loading the data.'),
            Gaps.h16,
            FilledButton(onPressed: widget.dataSource.refreshDatasource, child: const Text('Retry')),
          ],
        ),
      ),
      columns: <DataColumn2>[
        DataColumn2(label: const Text('Address'), size: ColumnSize.L, onSort: _sort),
        if (!widget.hideTierColumn) const DataColumn2(label: Text('Tier'), size: ColumnSize.S),
        DataColumn2(label: const Text('Created with Client'), onSort: _sort),
        DataColumn2(label: const Text('Number of Devices'), onSort: _sort),
        DataColumn2(label: const Text('Created at'), size: ColumnSize.S, onSort: _sort),
        DataColumn2(label: const Text('Last Login at'), size: ColumnSize.S, onSort: _sort),
        DataColumn2(label: const Text('Datawallet version'), onSort: _sort),
        DataColumn2(label: const Text('Identity Version'), onSort: _sort),
      ],
    );
  }

  void _setRowsPerPage(int? newValue) {
    _rowsPerPage = newValue ?? _rowsPerPage;
    widget.dataSource.refreshDatasource();
  }

  void _sort(int columnIndex, bool ascending) {
    setState(() {
      _sortColumnIndex = columnIndex;
      _sortColumnAscending = ascending;
    });

    widget.dataSource
      ..sort(sortColumnIndex: _sortColumnIndex, sortColumnAscending: _sortColumnAscending)
      ..refreshDatasource();
  }
}
