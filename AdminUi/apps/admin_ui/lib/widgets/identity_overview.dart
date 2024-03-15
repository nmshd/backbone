import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:admin_ui/widgets/helpers/identity_data_table_source.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';

class IdentityOverviewWidget extends StatefulWidget {
  const IdentityOverviewWidget({
    super.key,
  });

  @override
  State<IdentityOverviewWidget> createState() => IdentityOverviewWidgetState();
}

class IdentityOverviewWidgetState extends State<IdentityOverviewWidget> {
  final _scrollController = ScrollController();
  late IdentityDataTableSource dataSource;
  List<IdentityOverview> identities = [];
  int _columnIndex = 0;
  bool _columnAscending = true;

  @override
  void initState() {
    super.initState();
    dataSource = IdentityDataTableSource();
    loadData();
  }

  Future<void> loadData() async {
    final response = await GetIt.I.get<AdminApiClient>().identities.getIdentities();

    identities = response.data;
    dataSource.setData(identities, 0, columnAscending: true);
  }

  void _sort(int columnIndex, bool ascending) {
    setState(() {
      _columnIndex = columnIndex;
      _columnAscending = ascending;
      dataSource.setData(identities, _columnIndex, columnAscending: _columnAscending);
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scrollbar(
      controller: _scrollController,
      trackVisibility: true,
      thickness: 5,
      radius: const Radius.circular(2),
      child: SingleChildScrollView(
        scrollDirection: Axis.horizontal,
        controller: _scrollController,
        child: SizedBox(
          width: MediaQuery.of(context).size.width,
          child: PaginatedDataTable(
            header: const Text('Identities'),
            rowsPerPage: 5,
            sortColumnIndex: _columnIndex,
            sortAscending: _columnAscending,
            columnSpacing: 0,
            showEmptyRows: false,
            columns: <DataColumn>[
              DataColumn(
                label: const Text('Address'),
                onSort: _sort,
              ),
              DataColumn(
                label: const Text('Tier'),
                onSort: _sort,
              ),
              DataColumn(
                label: const Text('Created with Client'),
                onSort: _sort,
              ),
              DataColumn(
                label: const Text('Number of Devices'),
                onSort: _sort,
              ),
              DataColumn(
                label: const Text('Created at'),
                onSort: _sort,
              ),
              DataColumn(
                label: const Text('Last Login at'),
                onSort: _sort,
              ),
              DataColumn(
                label: const Text('Datawallet version'),
                onSort: _sort,
              ),
              DataColumn(
                label: const Text('Identity Version'),
                onSort: _sort,
              ),
            ],
            source: dataSource,
          ),
        ),
      ),
    );
  }
}
