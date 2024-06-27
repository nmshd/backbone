import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';

import '../../constants.dart';
import '../../extensions.dart';
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
      empty: Text(context.l10n.noEntitiesFound(context.l10n.identities)),
      errorBuilder: (error) => Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(context.l10n.error_data_load_failed),
            Gaps.h16,
            FilledButton(onPressed: widget.dataSource.refreshDatasource, child: Text(context.l10n.retry)),
          ],
        ),
      ),
      columns: <DataColumn2>[
        DataColumn2(label: Text(context.l10n.address), size: ColumnSize.L, onSort: _sort),
        if (!widget.hideTierColumn) DataColumn2(label: Text(context.l10n.tier), size: ColumnSize.S),
        DataColumn2(label: Text(context.l10n.created_with_client), onSort: _sort),
        DataColumn2(label: Text(context.l10n.number_of_devices), onSort: _sort),
        DataColumn2(label: Text(context.l10n.createdAt), size: ColumnSize.S, onSort: _sort),
        DataColumn2(label: Text(context.l10n.last_login_at), size: ColumnSize.S, onSort: _sort),
        DataColumn2(label: Text(context.l10n.datawallet_version), onSort: _sort),
        DataColumn2(label: Text(context.l10n.identity_version), onSort: _sort),
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
