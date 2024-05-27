import 'dart:io';

import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';

import '/core/core.dart';
import 'modals/modals.dart';

class TierDetail extends StatefulWidget {
  final String tierId;

  const TierDetail({required this.tierId, super.key});

  @override
  State<TierDetail> createState() => _TierDetailState();
}

class _TierDetailState extends State<TierDetail> {
  TierDetails? _tierDetails;
  late final ScrollController _scrollController;

  @override
  void initState() {
    super.initState();

    _scrollController = ScrollController();

    _reload();
  }

  @override
  void dispose() {
    _scrollController.dispose();

    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    if (_tierDetails == null) return const Center(child: CircularProgressIndicator());

    final tierDetails = _tierDetails!;
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
                        children: [
                          Text.rich(
                            TextSpan(
                              children: [
                                TextSpan(text: 'ID: ', style: Theme.of(context).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.bold)),
                                TextSpan(text: tierDetails.id, style: Theme.of(context).textTheme.bodyLarge),
                              ],
                            ),
                          ),
                          Text.rich(
                            TextSpan(
                              children: [
                                TextSpan(text: 'Name: ', style: Theme.of(context).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.bold)),
                                TextSpan(text: tierDetails.name, style: Theme.of(context).textTheme.bodyLarge),
                              ],
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                ),
              ],
            ),
            Gaps.h16,
            _QuotaList(tierDetails, _reload),
            Gaps.h16,
            _IdentitiesList(tierDetails),
          ],
        ),
      ),
    );
  }

  Future<void> _reload() async {
    final tierDetails = await GetIt.I.get<AdminApiClient>().tiers.getTier(widget.tierId);
    if (mounted) setState(() => _tierDetails = tierDetails.data);
  }
}

class _QuotaList extends StatefulWidget {
  final TierDetails tierDetails;
  final VoidCallback onQuotasChanged;

  const _QuotaList(this.tierDetails, this.onQuotasChanged);

  @override
  State<_QuotaList> createState() => _QuotaListState();
}

class _QuotaListState extends State<_QuotaList> {
  final List<String> _selectedQuotas = [];

  bool get isQueuedForDeletionTier => widget.tierDetails.id == 'TIR00000000000000001';

  @override
  Widget build(BuildContext context) {
    return Theme(
      data: Theme.of(context).copyWith(dividerColor: Colors.transparent),
      child: ExpansionTile(
        title: const Text('Quotas'),
        subtitle: isQueuedForDeletionTier
            ? const Text('View quotas for this tier. This tier is managed by the system and therefore read-only.')
            : const Text('View and assign quotas for this tier.'),
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
                          onPressed: () => showAddQuotaDialog(context: context, tierId: widget.tierDetails.id, onQuotaAdded: widget.onQuotasChanged),
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
                      DataColumn(label: Text('Max')),
                      DataColumn(label: Text('Period')),
                    ],
                    empty: const Text('No quotas added to this tier.'),
                    rows: widget.tierDetails.quotas
                        .map(
                          (quota) => DataRow2(
                            cells: [
                              DataCell(Text(quota.metric.displayName)),
                              DataCell(Text(quota.max.toString())),
                              DataCell(Text(quota.period)),
                            ],
                            onSelectChanged: widget.tierDetails.id == 'TIR00000000000000001' ? null : (_) => _toggleSelection(quota.id),
                            selected: _selectedQuotas.contains(quota.id),
                          ),
                        )
                        .toList(),
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
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
      message: 'Are you sure you want to remove the selected quotas from the tier "${widget.tierDetails.name}"?',
    );

    if (!confirmed) return;

    for (final quota in _selectedQuotas) {
      final result = await GetIt.I.get<AdminApiClient>().quotas.deleteTierQuota(tierId: widget.tierDetails.id, tierQuotaDefinitionId: quota);
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
          content: Text('Selected quotas have been removed.'),
          showCloseIcon: true,
        ),
      );
    }
  }
}

class _IdentitiesList extends StatefulWidget {
  final TierDetails tierDetails;

  const _IdentitiesList(this.tierDetails);

  @override
  State<_IdentitiesList> createState() => _IdentitiesListState();
}

class _IdentitiesListState extends State<_IdentitiesList> {
  late IdentityDataTableSource _dataSource;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();

    _dataSource = IdentityDataTableSource(
      locale: Localizations.localeOf(context),
      hideTierColumn: true,
      navigateToIdentity: ({required String address}) {
        context.push('/identities/$address');
      },
    );
  }

  @override
  void dispose() {
    _dataSource.dispose();

    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Theme(
      data: Theme.of(context).copyWith(dividerColor: Colors.transparent),
      child: ExpansionTile(
        title: const Text('Identities'),
        subtitle: const Text('View Identities associated with this Tier.'),
        children: [
          Card(
            child: Column(
              children: [
                IdentitiesFilter(
                  fixedTierId: widget.tierDetails.id,
                  onFilterChanged: ({IdentityOverviewFilter? filter}) async {
                    _dataSource
                      ..filter = filter
                      ..refreshDatasource();
                  },
                ),
                SizedBox(height: 500, child: IdentitiesDataTable(dataSource: _dataSource, hideTierColumn: true)),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
