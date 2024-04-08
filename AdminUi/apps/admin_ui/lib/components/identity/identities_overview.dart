import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:admin_ui/components/identity/identities_data_table_source.dart';
import 'package:admin_ui/components/identity/identities_filter.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';

class IdentitiesOverview extends StatefulWidget {
  const IdentitiesOverview({
    super.key,
  });

  @override
  State<IdentitiesOverview> createState() => _IdentitiesOverviewState();
}

class _IdentitiesOverviewState extends State<IdentitiesOverview> {
  late PaginatorController _paginatorController;
  late ScrollController _scrollController;
  late IdentityDataTableSource dataSource;

  List<IdentityOverview> identities = [];

  int _columnIndex = 0;
  bool _columnAscending = true;

  int _rowsPerPage = 5;
  int _currentPage = 0;

  @override
  void initState() {
    super.initState();
    _scrollController = ScrollController();
    _paginatorController = PaginatorController();
    dataSource = IdentityDataTableSource();
    loadIdentities();
  }

  @override
  void dispose() {
    _scrollController.dispose();
    _paginatorController.dispose();
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
              onFilterChanged: loadIdentities,
            ),
            Expanded(
              child: PaginatedDataTable2(
                rowsPerPage: _rowsPerPage,
                onRowsPerPageChanged: _setRowsPerPage,
                sortColumnIndex: _columnIndex,
                sortAscending: _columnAscending,
                showFirstLastButtons: true,
                columnSpacing: 5,
                source: dataSource,
                scrollController: _scrollController,
                isVerticalScrollBarVisible: true,
                renderEmptyRowsInTheEnd: false,
                controller: _paginatorController,
                availableRowsPerPage: const [5, 10, 25, 50, 100],
                onPageChanged: (newPage) {
                  _currentPage = newPage;
                  loadIdentities(pageNumber: newPage, pageSize: _rowsPerPage);
                },
                columns: <DataColumn2>[
                  DataColumn2(label: const Text('Address'), onSort: _sort, size: ColumnSize.L),
                  DataColumn2(label: const Text('Tier'), onSort: _sort, size: ColumnSize.S),
                  DataColumn2(label: const Text('Created with Client'), onSort: _sort),
                  DataColumn2(label: const Text('Number of Devices'), onSort: _sort),
                  DataColumn2(label: const Text('Created at'), onSort: _sort, size: ColumnSize.S),
                  DataColumn2(label: const Text('Last Login at'), onSort: _sort, size: ColumnSize.S),
                  DataColumn2(label: const Text('Datawallet version'), onSort: _sort, size: ColumnSize.S),
                  DataColumn2(label: const Text('Identity Version'), onSort: _sort, size: ColumnSize.S),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  void _setRowsPerPage(int? newValue) {
    setState(() {
      _rowsPerPage = newValue ?? _rowsPerPage;
    });
    loadIdentities(pageNumber: _currentPage, pageSize: _rowsPerPage);
  }

  void _sort(int columnIndex, bool ascending) {
    setState(() {
      _columnIndex = columnIndex;
      _columnAscending = ascending;
      dataSource.setData(identities, _columnIndex, columnAscending: _columnAscending);
    });
  }

  Future<void> loadIdentities({IdentityOverviewFilter? filter, int pageNumber = 0, int pageSize = 5}) async {
    final response = await GetIt.I.get<AdminApiClient>().identities.getIdentities(
          filter: filter,
          pageNumber: pageNumber,
          pageSize: pageSize,
        );

    if (response.hasData) {
      setState(() {
        identities = response.data;
        dataSource.setData(identities, _columnIndex, columnAscending: _columnAscending);
      });
    }
  }
}
