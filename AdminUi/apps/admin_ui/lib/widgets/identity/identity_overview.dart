import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:admin_ui/widgets/identity/helpers/identity_data_table_source.dart';
import 'package:admin_ui/widgets/identity/helpers/identity_filters.dart';
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
  final GlobalKey<ScaffoldMessengerState> _scaffoldKey = GlobalKey<ScaffoldMessengerState>();
  final _scrollController = ScrollController();
  late IdentityDataTableSource dataSource;

  List<IdentityOverview> identities = [];

  int _columnIndex = 0;
  bool _columnAscending = true;

  final _rowsPerPage = 1;

  @override
  void initState() {
    super.initState();
    dataSource = IdentityDataTableSource();
    loadIdentities();
  }

  void _sort(int columnIndex, bool ascending) {
    setState(() {
      _columnIndex = columnIndex;
      _columnAscending = ascending;
      dataSource.setData(identities, _columnIndex, columnAscending: _columnAscending);
    });
  }

  Future<void> loadIdentities({IdentityOverviewFilter? filter}) async {
    try {
      final response = await GetIt.I.get<AdminApiClient>().identities.getIdentities(filter: filter);

      setState(() {
        identities = response.data;
        dataSource.setData(identities, 0, columnAscending: true);
      });
    } catch (error) {
      showSnackbar(error as String);
    }
  }

  void showSnackbar(String error) {
    _scaffoldKey.currentState?.showSnackBar(
      SnackBar(
        content: Text('An error occurred while loading identities: $error'),
        backgroundColor: Colors.red,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(left: 10, right: 10),
      child: Column(
        children: [
          IdentityFilter(loadIdentities),
          Scrollbar(
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
                  rowsPerPage: _rowsPerPage,
                  availableRowsPerPage: const [1, 5, 10, 25, 100],
                  sortColumnIndex: _columnIndex,
                  sortAscending: _columnAscending,
                  showFirstLastButtons: true,
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
          ),
        ],
      ),
    );
  }
}
