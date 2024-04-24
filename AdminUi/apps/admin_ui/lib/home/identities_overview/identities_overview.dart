import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';

import '/core/core.dart';
import 'identities_data_table_source.dart';
import 'identities_filter.dart';

class IdentitiesOverview extends StatefulWidget {
  const IdentitiesOverview({super.key});

  @override
  State<IdentitiesOverview> createState() => _IdentitiesOverviewState();
}

class _IdentitiesOverviewState extends State<IdentitiesOverview> {
  late ScrollController _scrollController;
  late IdentityDataTableSource _dataSource;

  int _columnIndex = 0;
  bool _columnAscending = true;

  int _rowsPerPage = 5;

  @override
  void initState() {
    super.initState();
    _scrollController = ScrollController();
    _dataSource = IdentityDataTableSource();
  }

  @override
  void dispose() {
    _scrollController.dispose();
    _dataSource.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Padding(
          padding: EdgeInsets.only(left: 32),
          child: Text('A list of existing Identities'),
        ),
      ),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          mainAxisSize: MainAxisSize.min,
          children: [
            IdentitiesFilter(
              onFilterChanged: ({IdentityOverviewFilter? filter}) async {
                _dataSource
                  ..filter = filter
                  ..refreshDatasource();
              },
            ),
            Expanded(
              child: AsyncPaginatedDataTable2(
                rowsPerPage: _rowsPerPage,
                onRowsPerPageChanged: _setRowsPerPage,
                sortColumnIndex: _columnIndex,
                sortAscending: _columnAscending,
                showFirstLastButtons: true,
                columnSpacing: 5,
                source: _dataSource,
                scrollController: _scrollController,
                isVerticalScrollBarVisible: true,
                renderEmptyRowsInTheEnd: false,
                availableRowsPerPage: const [5, 10, 25, 50, 100],
                errorBuilder: (error) => Center(
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      const Text('An error occurred loading the data.'),
                      Gaps.h16,
                      FilledButton(onPressed: () => _dataSource.refreshDatasource(), child: const Text('Retry')),
                    ],
                  ),
                ),
                columns: <DataColumn2>[
                  DataColumn2(label: const Text('Address'), size: ColumnSize.L, onSort: _sort),
                  const DataColumn2(label: Text('Tier'), size: ColumnSize.S),
                  DataColumn2(label: const Text('Created with Client'), onSort: _sort),
                  DataColumn2(label: const Text('Number of Devices'), onSort: _sort),
                  DataColumn2(label: const Text('Created at'), size: ColumnSize.S, onSort: _sort),
                  DataColumn2(label: const Text('Last Login at'), size: ColumnSize.S, onSort: _sort),
                  DataColumn2(label: const Text('Datawallet version'), onSort: _sort),
                  DataColumn2(label: const Text('Identity Version'), onSort: _sort),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  void _setRowsPerPage(int? newValue) {
    _rowsPerPage = newValue ?? _rowsPerPage;
    _dataSource.refreshDatasource();
  }

  void _sort(int columnIndex, bool ascending) {
    setState(() {
      _columnIndex = columnIndex;
      _columnAscending = ascending;
    });
    _dataSource
      ..setData(_columnIndex, columnAscending: _columnAscending)
      ..refreshDatasource();
  }
}
