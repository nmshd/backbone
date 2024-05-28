import 'dart:io';

import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:intl/intl.dart';

import '/core/core.dart';
import 'modals/modals.dart';

class IdentityDetails extends StatefulWidget {
  final String address;

  const IdentityDetails({required this.address, super.key});

  @override
  State<IdentityDetails> createState() => _IdentityDetailsState();
}

class _IdentityDetailsState extends State<IdentityDetails> {
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
    if (_identityDetails == null || _tiers == null) return const Center(child: CircularProgressIndicator());

    final identityDetails = _identityDetails!;
    return Scrollbar(
      controller: _scrollController,
      child: SingleChildScrollView(
        controller: _scrollController,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (Platform.isMacOS || Platform.isWindows) const BackButton(),
            _IdentityDetailsCard(
              identityDetails: identityDetails,
              selectedTier: _selectedTier,
              onTierChanged: (String? newValue) {
                setState(() {
                  _selectedTier = newValue;
                });
              },
              availableTiers: _tiers!,
              updateTierOfIdentity: _reloadIdentity,
            ),
            Gaps.h16,
            _IdentityQuotaList(identityDetails, _reloadIdentity),
          ],
        ),
      ),
    );
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
    if (mounted) {
      setState(() => _tiers = tiers.data);
    }
  }
}

class _IdentityDetailsCard extends StatelessWidget {
  final Identity identityDetails;
  final String? selectedTier;
  final ValueChanged<String?>? onTierChanged;
  final List<TierOverview> availableTiers;
  final VoidCallback updateTierOfIdentity;

  const _IdentityDetailsCard({
    required this.identityDetails,
    required this.availableTiers,
    required this.updateTierOfIdentity,
    this.selectedTier,
    this.onTierChanged,
  });

  @override
  Widget build(BuildContext context) {
    final currentTier = availableTiers.firstWhere((tier) => tier.id == identityDetails.tierId);

    return Row(
      children: [
        Expanded(
          child: Card(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      Text(identityDetails.address, style: Theme.of(context).textTheme.headlineLarge),
                      Gaps.w16,
                      CopyToClipboardButton(
                        clipboardText: identityDetails.address,
                        successMessage: 'Identity address copied to clipboard.',
                      ),
                    ],
                  ),
                  const SizedBox(height: 32),
                  Wrap(
                    crossAxisAlignment: WrapCrossAlignment.center,
                    spacing: 8,
                    runSpacing: 8,
                    children: [
                      _IdentityDetails(
                        title: 'Client ID',
                        value: identityDetails.clientId,
                      ),
                      _IdentityDetails(
                        title: 'Public Key',
                        value: identityDetails.publicKey.ellipsize(20),
                        onIconPressed: () => context.setClipboardDataWithSuccessNotification(
                          clipboardText: identityDetails.publicKey,
                          successMessage: 'Public key copied to clipboard.',
                        ),
                        icon: Icons.copy,
                        tooltipMessage: 'Copy public key',
                      ),
                      _IdentityDetails(
                        title: 'Created at',
                        value:
                            '${DateFormat.yMd(Localizations.localeOf(context).languageCode).format(identityDetails.createdAt)} ${DateFormat.Hms().format(identityDetails.createdAt)}',
                      ),
                      _IdentityDetails(
                        title: 'Tier',
                        value: currentTier.name,
                        onIconPressed: currentTier.canBeManuallyAssigned
                            ? () => showChangeTierDialog(
                                  context: context,
                                  onTierUpdated: updateTierOfIdentity,
                                  identityDetails: identityDetails,
                                  availableTiers: availableTiers,
                                )
                            : null,
                        icon: Icons.edit,
                        tooltipMessage: 'Change tier',
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ),
        ),
      ],
    );
  }
}

class _IdentityDetails extends StatelessWidget {
  final String title;
  final String value;
  final VoidCallback? onIconPressed;
  final IconData? icon;
  final String? tooltipMessage;

  const _IdentityDetails({
    required this.title,
    required this.value,
    this.onIconPressed,
    this.icon,
    this.tooltipMessage,
  });

  @override
  Widget build(BuildContext context) {
    assert(
      onIconPressed == null || (onIconPressed != null && icon != null || tooltipMessage != null),
      'If edit is provided, icon and tooltipMessage must be provided too.',
    );

    return RawChip(
      label: Text.rich(
        TextSpan(
          children: [
            TextSpan(text: '$title ', style: Theme.of(context).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.bold)),
            TextSpan(text: value, style: Theme.of(context).textTheme.bodyLarge),
          ],
        ),
      ),
      onDeleted: onIconPressed,
      deleteIcon: Icon(icon),
      deleteButtonTooltipMessage: tooltipMessage,
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
          Card(
            child: Column(
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
                      DataColumn2(label: Text('Metric')),
                      DataColumn2(label: Text('Source'), size: ColumnSize.S),
                      DataColumn2(label: Text('Max'), size: ColumnSize.L),
                      DataColumn2(label: Text('Period'), size: ColumnSize.S),
                      DataColumn2(label: Text(''), size: ColumnSize.S),
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
                                  Text(
                                    quota.source,
                                    style: TextStyle(color: shouldDisable ? Colors.grey : null),
                                  ),
                                ),
                                DataCell(
                                  Row(
                                    children: [
                                      Text(
                                        '${quota.usage}/${quota.max}',
                                        style: TextStyle(color: shouldDisable ? Colors.grey : null),
                                      ),
                                      const SizedBox(width: 8),
                                      Expanded(
                                        child: LinearProgressIndicator(
                                          value: quota.max > 0 ? quota.usage / quota.max : 0,
                                          backgroundColor: shouldDisable ? Colors.grey : Theme.of(context).colorScheme.inversePrimary,
                                          valueColor:
                                              AlwaysStoppedAnimation<Color>(shouldDisable ? Colors.grey : Theme.of(context).colorScheme.primary),
                                          minHeight: 8,
                                        ),
                                      ),
                                    ],
                                  ),
                                ),
                                DataCell(
                                  Text(
                                    quota.period,
                                    style: TextStyle(color: shouldDisable ? Colors.grey : null),
                                  ),
                                ),
                                DataCell(
                                  Tooltip(
                                    message: tooltipMessage ?? '',
                                    child: isTierQuota
                                        ? Icon(
                                            Icons.info,
                                            color: shouldDisable ? Colors.grey : null,
                                          )
                                        : null,
                                  ),
                                ),
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
