import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:admin_ui/core/constants.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:intl/intl.dart';

import 'clients_filter.dart';
import 'modals/modals.dart';

class ClientsOverview extends StatefulWidget {
  const ClientsOverview({super.key});

  @override
  State<ClientsOverview> createState() => _ClientsOverviewState();
}

class _ClientsOverviewState extends State<ClientsOverview> {
  ClientsFilter _filter = ClientsFilter.empty;
  List<Clients> _originalClients = [];
  List<Clients> _filteredClients = [];
  final Set<String> _selectedClients = {};
  List<TierOverview> _defaultTiers = [];

  @override
  void initState() {
    super.initState();

    _reloadClients();
    _loadTiers();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('A list of existing Clients'),
      ),
      body: Card(
        child: Padding(
          padding: const EdgeInsets.all(8),
          child: SizedBox(
            height: double.infinity,
            child: Column(
              children: [
                ClientsFilterRow(
                  onFilterChanged: (filter) {
                    _filter = filter;
                    setState(() => _filteredClients = filter.apply(_originalClients));
                  },
                ),
                Gaps.h16,
                Row(
                  crossAxisAlignment: CrossAxisAlignment.end,
                  mainAxisAlignment: MainAxisAlignment.end,
                  children: [
                    IconButton(
                      icon: Icon(
                        Icons.delete,
                        color: _selectedClients.isNotEmpty ? Theme.of(context).colorScheme.onError : null,
                      ),
                      style: ButtonStyle(
                        backgroundColor: MaterialStateProperty.resolveWith((states) {
                          return _selectedClients.isNotEmpty ? Theme.of(context).colorScheme.error : null;
                        }),
                      ),
                      onPressed: _selectedClients.isNotEmpty
                          ? () => showRemoveClientsDialog(context: context, selectedClients: _selectedClients, onClientsRemoved: _reloadClients)
                          : null,
                    ),
                    Gaps.w8,
                    IconButton(
                      icon: Icon(
                        Icons.add,
                        color: Theme.of(context).colorScheme.onPrimary,
                      ),
                      style: ButtonStyle(
                        backgroundColor: MaterialStateProperty.resolveWith((states) {
                          return Theme.of(context).colorScheme.primary;
                        }),
                      ),
                      onPressed: () => showCreateClientDialog(context: context, defaultTiers: _defaultTiers, onClientCreated: _reloadClients),
                    ),
                  ],
                ),
                Expanded(
                  child: DataTable2(
                    isVerticalScrollBarVisible: true,
                    onSelectAll: (selected) {
                      if (selected == null) return;

                      setState(() {
                        if (selected) {
                          _selectedClients.addAll(_filteredClients.map((client) => client.clientId));
                        } else {
                          _selectedClients.clear();
                        }
                      });
                    },
                    columns: const <DataColumn2>[
                      DataColumn2(label: Text('Client ID'), size: ColumnSize.L),
                      DataColumn2(label: Text('Display Name'), size: ColumnSize.L),
                      DataColumn2(label: Text('Default Tier')),
                      DataColumn2(label: Text('Number of Identities'), size: ColumnSize.L),
                      DataColumn2(label: Text('Created At')),
                      DataColumn2(label: Text(''), size: ColumnSize.L),
                    ],
                    rows: _filteredClients
                        .map(
                          (client) => DataRow2(
                            selected: _selectedClients.contains(client.clientId),
                            onSelectChanged: (selected) {
                              if (selected == null) return;

                              setState(() {
                                if (selected) {
                                  _selectedClients.add(client.clientId);
                                } else {
                                  _selectedClients.remove(client.clientId);
                                }
                              });
                            },
                            cells: [
                              DataCell(Text(client.clientId)),
                              DataCell(Text(client.displayName)),
                              DataCell(Text(client.defaultTier.name)),
                              DataCell(Text('${client.numberOfIdentities}')),
                              DataCell(Text(DateFormat('yyyy-MM-dd').format(client.createdAt))),
                              DataCell(
                                ElevatedButton(
                                  style: ButtonStyle(backgroundColor: MaterialStateProperty.all<Color>(Theme.of(context).colorScheme.primary)),
                                  onPressed: () => showChangeClientSecretDialog(context: context, clientId: client.clientId),
                                  child: Text(
                                    'Change Client Secret',
                                    style: TextStyle(color: Theme.of(context).colorScheme.onPrimary),
                                    textAlign: TextAlign.center,
                                  ),
                                ),
                              ),
                            ],
                          ),
                        )
                        .toList(),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Future<void> _reloadClients() async {
    final response = await GetIt.I.get<AdminApiClient>().clients.getClients();
    if (mounted) {
      setState(() {
        _originalClients = response.data;
        _filteredClients = _filter.apply(response.data);
      });
    }
  }

  Future<void> _loadTiers() async {
    final response = await GetIt.I.get<AdminApiClient>().tiers.getTiers();
    setState(() {
      _defaultTiers = response.data.where((element) => element.canBeUsedAsDefaultForClient == true).toList();
    });
  }
}
