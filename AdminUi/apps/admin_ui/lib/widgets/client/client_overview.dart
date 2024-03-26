import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:admin_ui/widgets/client/helpers/client_data_table_source.dart';
import 'package:admin_ui/widgets/client/helpers/client_filters.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';

class ClientOverview extends StatefulWidget {
  const ClientOverview({super.key});

  @override
  State<ClientOverview> createState() => _ClientOverviewState();
}

class _ClientOverviewState extends State<ClientOverview> {
  late List<Clients> clients;
  late List<bool> rowCheckboxState;
  bool selectAll = false;

  late ClientDataTableSource _clientDataTableSource;

  bool sortAscending = true;
  int sortColumnIndex = 0;

  @override
  void initState() {
    super.initState();
    clients = [];
    rowCheckboxState = [];
    loadClients().then((_) {
      setState(() {
        rowCheckboxState = List.generate(clients.length, (_) => false);
      });
    });
    _clientDataTableSource = ClientDataTableSource(
      clients,
      rowCheckboxState,
      _handleRowTap,
      sortColumnIndex,
      sortAscending: sortAscending,
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Clients'),
        centerTitle: true,
        bottom: const PreferredSize(
          preferredSize: Size.fromHeight(20),
          child: Text(
            'Subtitle Here',
            style: TextStyle(fontSize: 14),
          ),
        ),
      ),
      body: Column(
        children: [
          ClientFilter((List<Clients> filteredClients) {
            clients = filteredClients;
          }),
          const SizedBox(
            height: 8,
          ),
          SizedBox(
            width: MediaQuery.of(context).size.width,
            child: PaginatedDataTable(
              onSelectAll: (selectAll) {},
              actions: [
                IconButton(
                  onPressed: () {},
                  icon: const Icon(Icons.delete),
                ),
                IconButton(
                  onPressed: () {},
                  icon: const Icon(Icons.add),
                ),
              ],
              header: const Text(''),
              showEmptyRows: false,
              sortColumnIndex: sortColumnIndex,
              sortAscending: sortAscending,
              showFirstLastButtons: true,
              columns: <DataColumn>[
                DataColumn(
                  label: const Center(
                    child: Text('Client ID'),
                  ),
                  onSort: (columnIndex, ascending) {
                    setState(() {
                      sortColumnIndex = columnIndex;
                      sortAscending = ascending;
                    });
                    _clientDataTableSource.onSort(columnIndex, ascending: ascending);
                  },
                ),
                DataColumn(
                  label: const Center(
                    child: Text('Display Name'),
                  ),
                  onSort: (columnIndex, ascending) {
                    setState(() {
                      sortColumnIndex = columnIndex;
                      sortAscending = ascending;
                    });
                    _clientDataTableSource.onSort(columnIndex, ascending: ascending);
                  },
                ),
                const DataColumn(
                  label: Center(
                    child: Text('Default Tier'),
                  ),
                ),
                const DataColumn(
                  label: Center(
                    child: Text('Number of Identities'),
                  ),
                ),
                const DataColumn(
                  label: Center(
                    child: Text('Created At'),
                  ),
                ),
                const DataColumn(
                  label: Center(
                    child: Text(''),
                  ),
                ),
              ],
              rowsPerPage: 5,
              source: _clientDataTableSource,
            ),
          ),
        ],
      ),
    );
  }

  Future<void> loadClients() async {
    final response = await GetIt.instance.get<AdminApiClient>().clients.getClients();

    setState(() {
      clients = response.data;
      _clientDataTableSource = ClientDataTableSource(
        clients,
        rowCheckboxState,
        _handleRowTap,
        sortColumnIndex,
        sortAscending: sortAscending,
      );
    });
  }

  void _handleRowTap(Clients client) {
    // TODO(stamenione): handle the navigation to client detail page
  }
}
