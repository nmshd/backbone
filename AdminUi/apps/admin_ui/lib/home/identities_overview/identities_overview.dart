import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';

import '/core/core.dart';
import 'identities_data_table_source.dart';
import 'identities_filter.dart';

class IdentitiesOverview extends StatefulWidget {
  const IdentitiesOverview({
    super.key,
  });

  @override
  State<IdentitiesOverview> createState() => _IdentitiesOverviewState();
}

class _IdentitiesOverviewState extends State<IdentitiesOverview> {
  late ScrollController _scrollController;
  late IdentityDataTableSource _dataSource;

  List<IdentityOverview> _identities = [];
  late Pagination _pagination;

  int _columnIndex = 0;
  bool _columnAscending = true;

  int _rowsPerPage = 5;
  int _currentPage = 0;

  @override
  void initState() {
    super.initState();
    _scrollController = ScrollController();
    _dataSource = IdentityDataTableSource();
    _loadIdentities();
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
              onFilterChanged: _loadIdentities,
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
                onPageChanged: (newPage) {
                  _currentPage = newPage;
                  _loadIdentities();
                },
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
                  DataColumn2(label: const Text('Address'), onSort: _sort, size: ColumnSize.L),
                  DataColumn2(label: const Text('Tier'), onSort: _sort, size: ColumnSize.S),
                  DataColumn2(label: const Text('Created with Client'), onSort: _sort),
                  DataColumn2(label: const Text('Number of Devices'), onSort: _sort),
                  DataColumn2(label: const Text('Created at'), onSort: _sort, size: ColumnSize.S),
                  DataColumn2(label: const Text('Last Login at'), onSort: _sort, size: ColumnSize.S),
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
    _columnIndex = columnIndex;
    _columnAscending = ascending;
    _dataSource
      ..setData(_identities, _pagination, _columnIndex, columnAscending: _columnAscending)
      ..refreshDatasource();
  }

  Future<void> _loadIdentities({IdentityOverviewFilter? filter}) async {
    final response = await GetIt.I.get<AdminApiClient>().identities.getIdentities(
          filter: filter,
          pageNumber: _currentPage,
          pageSize: _rowsPerPage,
        );

    if (!mounted) return;

    _identities = response.data;
    _pagination = response.pagination;
    _dataSource
      ..setData(_identities, _pagination, _columnIndex, columnAscending: _columnAscending)
      ..refreshDatasource();
  }
}
