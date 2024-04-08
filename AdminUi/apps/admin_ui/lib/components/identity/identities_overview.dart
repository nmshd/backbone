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
  late ScrollController _scrollController;
  late IdentityDataTableSource dataSource;

  List<IdentityOverview> identities = [];
  late Pagination pagination;

  int _columnIndex = 0;
  bool _columnAscending = true;

  int _rowsPerPage = 5;
  int _currentPage = 0;

  @override
  void initState() {
    super.initState();
    _scrollController = ScrollController();
    dataSource = IdentityDataTableSource();
    loadIdentities();
  }

  @override
  void dispose() {
    _scrollController.dispose();
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
                availableRowsPerPage: const [5, 10, 25, 50, 100],
                onPageChanged: (newPage) {
                  _currentPage = newPage;
                  loadIdentities();
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
  }

  void _sort(int columnIndex, bool ascending) {
    setState(() {
      _columnIndex = columnIndex;
      _columnAscending = ascending;
      dataSource.setData(identities, pagination, _columnIndex, columnAscending: _columnAscending);
    });
  }

  Future<void> loadIdentities({IdentityOverviewFilter? filter}) async {
    print('loadIdentities called with page $_currentPage and filter $filter');

    final response = await GetIt.I.get<AdminApiClient>().identities.getIdentities(
          filter: filter,
          pageNumber: _currentPage,
          pageSize: _rowsPerPage,
        );

    if (!mounted) return; // Check if the widget is still in the widget tree

    setState(() {
      identities = response.data;
      pagination = response.pagination;
      dataSource.setData(identities, pagination, _columnIndex, columnAscending: _columnAscending);
    });
  }
}
