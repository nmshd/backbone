import 'dart:io';

import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:admin_ui/core/core.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:intl/intl.dart';

import 'modals/modals.dart';

class IdentityDetails extends StatefulWidget {
  final String address;

  const IdentityDetails({required this.address, super.key});

  @override
  State<IdentityDetails> createState() => _IdentityDetailsState();
}

class _IdentityDetailsState extends State<IdentityDetails> {
  static const noTiersFoundMessage = 'No tiers found.';

  Identity? _identityDetails;
  List<TierOverview>? _tiers;
  String? _selectedTier;

  late final ScrollController _scrollController;

  @override
  void initState() {
    super.initState();

    _scrollController = ScrollController();

    _reloadIdentity();
    _reloadTiers();
  }

  @override
  void dispose() {
    _scrollController.dispose();

    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    if (_identityDetails == null) return const Center(child: CircularProgressIndicator());

    final identityDetails = _identityDetails!;
    return Scrollbar(
      controller: _scrollController,
      child: SingleChildScrollView(
        controller: _scrollController,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (Platform.isMacOS || Platform.isWindows) const BackButton(),
            Row(
              children: [
                Expanded(
                  child: Card(
                    child: Padding(
                      padding: const EdgeInsets.all(16),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          const Text(
                            'Identities Overview',
                            style: TextStyle(fontSize: 40),
                          ),
                          Gaps.h32,
                          Row(
                            children: [
                              _IdentityDetailsColumn(
                                columnTitle: 'Address',
                                columnValue: _identityDetails!.address,
                              ),
                              Gaps.w16,
                              _IdentityDetailsColumn(
                                columnTitle: 'Client ID',
                                columnValue: _identityDetails!.clientId,
                              ),
                              Gaps.w16,
                              _IdentityDetailsColumn(
                                columnTitle: 'Public Key',
                                columnValue: _identityDetails!.publicKey,
                              ),
                              Gaps.w16,
                              _IdentityDetailsColumn(
                                columnTitle: 'Created at',
                                columnValue: DateFormat('yyyy-MM-dd hh:MM:ss').format(identityDetails.createdAt),
                              ),
                              Gaps.w16,
                              Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text(
                                    'Tier',
                                    style: Theme.of(context).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.bold, fontSize: 20),
                                  ),
                                  DropdownButton<String>(
                                    isDense: true,
                                    value: _selectedTier,
                                    onChanged: (String? newValue) {
                                      setState(() {
                                        _selectedTier = newValue;
                                      });
                                      _updateIdentity();
                                    },
                                    items: _tiers!.isNotEmpty
                                        ? _tiers!.where(_isTierManuallyAssignable).map((tier) {
                                            final isDisabled = _isTierDisabled(tier);
                                            return DropdownMenuItem<String>(
                                              value: tier.id,
                                              enabled: !isDisabled,
                                              child: isDisabled
                                                  ? Text(
                                                      tier.name,
                                                      style: TextStyle(
                                                        color: Theme.of(context).disabledColor,
                                                        fontSize: 18,
                                                      ),
                                                    )
                                                  : Text(
                                                      tier.name,
                                                      style: const TextStyle(
                                                        fontSize: 18,
                                                      ),
                                                    ),
                                            );
                                          }).toList()
                                        : [
                                            const DropdownMenuItem<String>(
                                              value: noTiersFoundMessage,
                                              child: Text(
                                                noTiersFoundMessage,
                                                style: TextStyle(
                                                  fontSize: 14,
                                                ),
                                              ),
                                            ),
                                          ],
                                  ),
                                ],
                              ),
                            ],
                          ),
                        ],
                      ),
                    ),
                  ),
                ),
              ],
            ),
            _IdentityQuotaList(identityDetails, _reloadIdentity),
          ],
        ),
      ),
    );
  }

  bool _isTierDisabled(TierOverview tier) {
    if (_tiers == null || _identityDetails == null) {
      return false;
    }
    final tiersThatCannotBeUnassigned = _tiers!.where((t) => !t.canBeManuallyAssigned);
    final identityIsInTierThatCannotBeUnassigned = tiersThatCannotBeUnassigned.any((t) => t.id == _identityDetails!.tierId);
    return identityIsInTierThatCannotBeUnassigned && tier.id != _identityDetails!.tierId;
  }

  bool _isTierManuallyAssignable(TierOverview tier) {
    return tier.canBeManuallyAssigned || tier.id == _identityDetails!.tierId;
  }

  Future<void> _updateIdentity() async {
    if (_identityDetails == null) return;

    final scaffoldMessenger = ScaffoldMessenger.of(context);

    try {
      await GetIt.I.get<AdminApiClient>().identities.updateIdentity(_identityDetails!.address, tierId: _selectedTier!);

      scaffoldMessenger.showSnackBar(
        const SnackBar(
          content: Text('Identity updated successfully. Reloading..'),
          duration: Duration(seconds: 3),
        ),
      );

      await _reloadIdentity();
    } catch (e) {
      scaffoldMessenger.showSnackBar(
        const SnackBar(
          content: Text('Failed to update identity. Please try again.'),
          duration: Duration(seconds: 3),
        ),
      );
    }
  }

  Future<void> _reloadIdentity() async {
    final identityDetails = await GetIt.I.get<AdminApiClient>().identities.getIdentity(widget.address);
    if (mounted) {
      setState(() {
        _identityDetails = identityDetails.data;
        _selectedTier = _identityDetails!.tierId;
      });
    }
  }

  Future<void> _reloadTiers() async {
    final tiers = await GetIt.I.get<AdminApiClient>().tiers.getTiers();
    if (mounted) setState(() => _tiers = tiers.data);
  }
}

class _IdentityDetailsColumn extends StatelessWidget {
  final String columnTitle;
  final String columnValue;

  const _IdentityDetailsColumn({required this.columnTitle, required this.columnValue});

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          columnTitle,
          style: Theme.of(context).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.bold, fontSize: 20),
        ),
        Text(
          columnValue,
          style: const TextStyle(fontSize: 18),
        ),
      ],
    );
  }
}

class _IdentityQuotaList extends StatefulWidget {
  final Identity identityDetails;
  final VoidCallback onQuotasChanged;

  const _IdentityQuotaList(this.identityDetails, this.onQuotasChanged);

  @override
  State<_IdentityQuotaList> createState() => _IdentityQuotaListState();
}

class _IdentityQuotaListState extends State<_IdentityQuotaList> {
  final List<String> _selectedQuotas = [];

  bool get isQueuedForDeletionTier => widget.identityDetails.tierId == 'TIR00000000000000001';

  @override
  Widget build(BuildContext context) {
    final groupedQuotas = _groupQuotas();

    return Theme(
      data: Theme.of(context).copyWith(dividerColor: Colors.transparent),
      child: ExpansionTile(
        title: const Text('Quotas'),
        subtitle: const Text('View and assign quotas for this identity.'),
        children: [
          Column(
            children: [
              if (!isQueuedForDeletionTier)
                Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.end,
                    children: [
                      IconButton(
                        icon: Icon(
                          Icons.delete,
                          color: _selectedQuotas.isNotEmpty ? Theme.of(context).colorScheme.onError : null,
                        ),
                        style: ButtonStyle(
                          backgroundColor: WidgetStateProperty.resolveWith((states) {
                            return _selectedQuotas.isNotEmpty ? Theme.of(context).colorScheme.error : null;
                          }),
                        ),
                        onPressed: _selectedQuotas.isNotEmpty ? _removeSelectedQuotas : null,
                      ),
                      Gaps.w8,
                      IconButton.filled(
                        icon: const Icon(Icons.add),
                        onPressed: () => showAddIdentityQuotaDialog(
                          context: context,
                          address: widget.identityDetails.address,
                          onQuotaAdded: widget.onQuotasChanged,
                        ),
                      ),
                    ],
                  ),
                ),
              SizedBox(
                width: double.infinity,
                height: 500,
                child: DataTable2(
                  columns: const [
                    DataColumn(label: Text('Metric')),
                    DataColumn(label: Text('Source')),
                    DataColumn(label: Text('Max')),
                    DataColumn(label: Text('Period')),
                  ],
                  empty: const Text('No quotas applied for this identity.'),
                  rows: groupedQuotas.entries.expand((entry) {
                    final metricName = entry.key;
                    final quotas = entry.value;

                    final hasIndividualQuota = quotas.any((quota) => quota.source == 'Individual');

                    return [
                      DataRow2(
                        color: WidgetStateProperty.all(Theme.of(context).colorScheme.surfaceBright),
                        cells: [
                          DataCell(Text(metricName)),
                          const DataCell(Text('')),
                          const DataCell(Text('')),
                          const DataCell(Text('')),
                        ],
                      ),
                      ...quotas.map(
                        (quota) {
                          final isTierQuota = quota.source == 'Tier';
                          final shouldDisable = isTierQuota && hasIndividualQuota;
                          final tooltipMessage = shouldDisable ? 'Tier quotas do not take effect if there is an individual quota.' : null;

                          return DataRow2(
                            selected: _selectedQuotas.contains(quota.id),
                            color: shouldDisable ? WidgetStateProperty.all(Theme.of(context).colorScheme.surfaceBright) : null,
                            onSelectChanged: shouldDisable || isTierQuota ? null : (_) => _toggleSelection(quota.id),
                            cells: [
                              DataCell(Container()),
                              DataCell(
                                Tooltip(
                                  message: tooltipMessage ?? '',
                                  child: Text(quota.source),
                                ),
                              ),
                              DataCell(
                                Row(
                                  children: [
                                    Text('${quota.usage}/${quota.max}'),
                                    const SizedBox(width: 8),
                                    Expanded(
                                      child: LinearProgressIndicator(
                                        value: quota.max > 0 ? quota.usage / quota.max : 0,
                                        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
                                        valueColor: AlwaysStoppedAnimation<Color>(Theme.of(context).colorScheme.primary),
                                        minHeight: 8,
                                      ),
                                    ),
                                  ],
                                ),
                              ),
                              DataCell(Text(quota.period)),
                            ],
                          );
                        },
                      ),
                    ];
                  }).toList(),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Map<String, List<IdentityQuota>> _groupQuotas() {
    final groupedQuotas = <String, List<IdentityQuota>>{};

    if (widget.identityDetails.quotas != null) {
      for (final quota in widget.identityDetails.quotas!) {
        if (groupedQuotas.containsKey(quota.metric.displayName)) {
          groupedQuotas[quota.metric.displayName]!.add(quota);
        } else {
          groupedQuotas[quota.metric.displayName] = [quota];
        }
      }
    }

    return groupedQuotas;
  }

  void _toggleSelection(String id) {
    setState(() {
      if (_selectedQuotas.contains(id)) {
        _selectedQuotas.remove(id);
        return;
      }

      _selectedQuotas.add(id);
    });
  }

  Future<void> _removeSelectedQuotas() async {
    final confirmed = await showConfirmationDialog(
      context: context,
      title: 'Remove Quotas',
      message: 'Are you sure you want to remove the selected quotas from the identity "${widget.identityDetails.address}"?',
    );

    if (!confirmed) return;

    for (final quota in _selectedQuotas) {
      final result =
          await GetIt.I.get<AdminApiClient>().quotas.deleteIdentityQuota(address: widget.identityDetails.address, individualQuotaId: quota);
      if (result.hasError && mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('An error occurred while deleting the quota(s). Please try again.'),
            showCloseIcon: true,
          ),
        );
        return;
      }

      widget.onQuotasChanged();
    }

    _selectedQuotas.clear();
    if (mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Selected quota(s) have been removed.'),
          showCloseIcon: true,
        ),
      );
    }
  }
}
