import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:intl/intl.dart';

import '/core/core.dart';
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
      appBar: AppBar(title: Text(context.l10n.list_of_all_clients)),
      body: Card(
        child: Padding(
          padding: const EdgeInsets.all(8),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              ClientsFilterRow(onFilterChanged: (filter) => setState(() => _filter = filter)),
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
                      backgroundColor: WidgetStateProperty.resolveWith((states) {
                        return _selectedClients.isNotEmpty ? Theme.of(context).colorScheme.error : null;
                      }),
                    ),
                    onPressed: _selectedClients.isNotEmpty ? _removeSelectedClients : null,
                  ),
                  Gaps.w8,
                  IconButton.filled(
                    icon: const Icon(Icons.add),
                    onPressed: () => showCreateClientDialog(context: context, defaultTiers: _defaultTiers, onClientCreated: _reloadClients),
                  ),
                ],
              ),
              Expanded(
                child: DataTable2(
                  isVerticalScrollBarVisible: true,
                  empty: Text(context.l10n.no_clients_found),
                  onSelectAll: (selected) {
                    if (selected == null) return;

                    setState(() {
                      if (selected) {
                        _selectedClients.addAll(_originalClients.where((e) => _filter.matches(e)).map((client) => client.clientId));
                      } else {
                        _selectedClients.clear();
                      }
                    });
                  },
                  columns: <DataColumn2>[
                    DataColumn2(label: Text(context.l10n.client_ID), size: ColumnSize.L),
                    DataColumn2(label: Text(context.l10n.display_name), size: ColumnSize.L),
                    DataColumn2(label: Text(context.l10n.default_tier)),
                    DataColumn2(label: Text(context.l10n.number_of_identities), size: ColumnSize.L),
                    DataColumn2(label: Text(context.l10n.createdAt)),
                    const DataColumn2(label: Text(''), size: ColumnSize.L),
                  ],
                  rows: _originalClients
                      .where((e) => _filter.matches(e))
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
                            DataCell(
                              Tooltip(
                                message:
                                    '${DateFormat.yMd(Localizations.localeOf(context).languageCode).format(client.createdAt)} ${DateFormat.Hms().format(client.createdAt)}',
                                child: Text(DateFormat.yMd(Localizations.localeOf(context).languageCode).format(client.createdAt)),
                              ),
                            ),
                            DataCell(
                              ElevatedButton(
                                style: ButtonStyle(backgroundColor: WidgetStateProperty.all<Color>(Theme.of(context).colorScheme.primary)),
                                onPressed: () => showChangeClientSecretDialog(context: context, clientId: client.clientId),
                                child: Text(
                                  context.l10n.change_client_secret,
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
    );
  }

  Future<void> _reloadClients() async {
    final response = await GetIt.I.get<AdminApiClient>().clients.getClients();
    if (mounted) setState(() => _originalClients = response.data);
  }

  Future<void> _loadTiers() async {
    final response = await GetIt.I.get<AdminApiClient>().tiers.getTiers();
    setState(() => _defaultTiers = response.data.where((element) => element.canBeUsedAsDefaultForClient == true).toList());
  }

  Future<void> _removeSelectedClients() async {
    final confirmed = await showConfirmationDialog(
      context: context,
      title: context.l10n.remove_clients,
      message: context.l10n.remove_clients_message,
    );

    if (!confirmed) return;

    for (final clientId in _selectedClients) {
      final result = await GetIt.I.get<AdminApiClient>().clients.deleteClient(clientId);
      if (result.hasError && mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(context.l10n.error_deleting_client),
            showCloseIcon: true,
          ),
        );
        return;
      }

      await _reloadClients();
    }

    _selectedClients.clear();

    if (mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(context.l10n.selected_client_removed),
          showCloseIcon: true,
        ),
      );
    }
  }
}
