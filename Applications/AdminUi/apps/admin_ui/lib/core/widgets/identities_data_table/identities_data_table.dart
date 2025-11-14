import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';

import '../../extensions.dart';
import 'identities_data_table_source.dart';

export 'identities_data_table_source.dart';
export 'identities_filter.dart';

class IdentitiesDataTable extends StatefulWidget {
  final IdentityDataTableSource dataSource;
  final bool hideClientColumn;
  final bool hideTierColumn;

  const IdentitiesDataTable({required this.dataSource, this.hideTierColumn = false, this.hideClientColumn = false, super.key});

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
      wrapInCard: false,
      empty: Text(context.l10n.identitiesDataTable_noIdentitiesFound),
      errorBuilder: (error) => Center(
        child: Column(
          mainAxisSize: .min,
          spacing: 16,
          children: [
            Text(context.l10n.identitiesDataTable_failedToLoadData),
            FilledButton(onPressed: widget.dataSource.refreshDatasource, child: Text(context.l10n.retry)),
          ],
        ),
      ),
      columns: <DataColumn2>[
        DataColumn2(label: Text(context.l10n.address), size: .L, onSort: _sort),
        if (!widget.hideTierColumn) DataColumn2(label: Text(context.l10n.tier), size: .S),
        if (!widget.hideClientColumn) DataColumn2(label: Text(context.l10n.identitiesDataTable_createdWithClient), onSort: _sort),
        DataColumn2(label: Text(context.l10n.numberOfDevices), onSort: _sort),
        DataColumn2(label: Text(context.l10n.createdAt), size: .S, onSort: _sort),
        DataColumn2(label: Text(context.l10n.lastLoginAt), size: .S, onSort: _sort),
        DataColumn2(label: Text(context.l10n.datawalletVersion), onSort: _sort),
        DataColumn2(label: Text(context.l10n.identityVersion), onSort: _sort),
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
