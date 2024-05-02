import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:admin_ui/core/constants.dart';
import 'package:admin_ui/home/clients_overview/clients_filter.dart';
import 'package:admin_ui/home/clients_overview/clients_overview_dialogs/clients_overview_dialogs.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';

class ClientsOverview extends StatefulWidget {
  const ClientsOverview({super.key});

  @override
  State<ClientsOverview> createState() => _ClientsOverviewState();
}

class _ClientsOverviewState extends State<ClientsOverview> {
  late ScrollController _scrollController;

  late List<Clients> _clients;
  late List<Clients> _originalClients;
  late Map<String, bool> _selectedClients;

  late List<TierOverview> _defaultTiers;

  @override
  void initState() {
    super.initState();
    _scrollController = ScrollController();
    _clients = [];
    _originalClients = [];
    _selectedClients = {};
    _defaultTiers = [];
    loadClients();
    loadTiers();
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
                ClientsFilter(
                  loadedClients: _originalClients,
                  onFilterChanged: (filteredClients) {
                    setState(() {
                      _clients = filteredClients;
                    });
                  },
                ),
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
                          ? () {
                              showDialog<void>(
                                context: context,
                                builder: (BuildContext dialogContext) {
                                  return RemoveClientsDialog(
                                    selectedClients: _selectedClients,
                                    loadClients: loadClients,
                                    onSuccess: (numberOfRemovedClients) {
                                      if (numberOfRemovedClients == 1) {
                                        _showSnackBar(context, 'Successfully removed $numberOfRemovedClients Client.');
                                      } else {
                                        _showSnackBar(context, 'Successfully removed $numberOfRemovedClients Clients.');
                                      }
                                    },
                                  );
                                },
                              );
                            }
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
                      onPressed: () {
                        showDialog<void>(
                          context: context,
                          builder: (BuildContext context) => CreateClientDialog(defaultTiers: _defaultTiers, loadClients: loadClients),
                        );
                      },
                    ),
                  ],
                ),
                Expanded(
                  child: DataTable2(
                    isVerticalScrollBarVisible: true,
                    scrollController: _scrollController,
                    onSelectAll: (selected) {
                      setState(() {
                        for (final client in _clients) {
                          if (selected!) {
                            _selectedClients[client.clientId] = selected;
                          } else {
                            _selectedClients.remove(client.clientId);
                          }
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
                    rows: _clients
                        .map(
                          (client) => DataRow2(
                            selected: _selectedClients[client.clientId] ?? false,
                            onSelectChanged: (selected) {
                              setState(() {
                                if (selected!) {
                                  _selectedClients[client.clientId] = selected;
                                } else {
                                  _selectedClients.remove(client.clientId);
                                }
                              });
                            },
                            onTap: () {},
                            cells: [
                              DataCell(Text(client.clientId)),
                              DataCell(Text(client.displayName)),
                              DataCell(Text(client.defaultTier.name)),
                              DataCell(Text('${client.numberOfIdentities}')),
                              DataCell(Text(client.createdAt.toIso8601String().substring(0, 10))),
                              DataCell(
                                ElevatedButton(
                                  style: ButtonStyle(backgroundColor: MaterialStateProperty.all<Color>(Theme.of(context).colorScheme.primary)),
                                  onPressed: () {
                                    showDialog<void>(
                                      context: context,
                                      builder: (BuildContext context) =>
                                          ChangeClientSecretDialog(clientId: client.clientId, loadClients: loadClients),
                                    );
                                  },
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

  void _showSnackBar(BuildContext context, String message) {
    final snackBar = SnackBar(content: Text(message));
    ScaffoldMessenger.of(context).showSnackBar(snackBar);
  }

  Future<void> loadClients() async {
    try {
      final response = await GetIt.I.get<AdminApiClient>().clients.getClients();
      if (mounted) {
        setState(() {
          _originalClients = response.data;
          _clients = response.data;
        });
      }
    } catch (e) {
      if (mounted) {
        setState(() {
          _originalClients = [];
          _clients = [];
        });
      }
    }
  }

  Future<void> loadTiers() async {
    final response = await GetIt.I.get<AdminApiClient>().tiers.getTiers();
    setState(() {
      _defaultTiers = response.data.where((element) => element.canBeUsedAsDefaultForClient == true).toList();
    });
  }
}
