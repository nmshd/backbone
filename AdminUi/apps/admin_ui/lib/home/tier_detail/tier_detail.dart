import 'dart:io';

import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';

import '/core/core.dart';

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
                                TextSpan(
                                    text: '${context.l10n.id}: ',
                                    style: Theme.of(context).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.bold)),
                                TextSpan(text: tierDetails.id, style: Theme.of(context).textTheme.bodyLarge),
                              ],
                            ),
                          ),
                          Text.rich(
                            TextSpan(
                              children: [
                                TextSpan(
                                  text: '${context.l10n.name}: ',
                                  style: Theme.of(context).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.bold),
                                ),
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
        title: Text(context.l10n.quotas),
        subtitle: isQueuedForDeletionTier ? Text(context.l10n.view_quotas_for_the_tier) : Text(context.l10n.viewAndAssignQuotasForEntity('tier')),
        children: [
          Card(
            child: Column(
              children: [
                if (!isQueuedForDeletionTier)
                  QuotasButtonGroup(
                    selectedQuotas: _selectedQuotas,
                    onQuotasChanged: widget.onQuotasChanged,
                    tierId: widget.tierDetails.id,
                  ),
                SizedBox(
                  width: double.infinity,
                  height: 500,
                  child: DataTable2(
                    columns: [
                      DataColumn(label: Text(context.l10n.metric)),
                      DataColumn(label: Text(context.l10n.max)),
                      DataColumn(label: Text(context.l10n.period)),
                    ],
                    empty: Text(context.l10n.no_quotas_added_for_tier),
                    rows: widget.tierDetails.quotas
                        .map(
                          (quota) => DataRow2(
                            cells: [
                              DataCell(Text(quota.metric.displayName)),
                              DataCell(Text(quota.max.toString())),
                              DataCell(Text(quota.period)),
                            ],
                            onSelectChanged: isQueuedForDeletionTier ? null : (_) => _toggleSelection(quota.id),
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
        title: Text(context.l10n.identities),
        subtitle: Text(context.l10n.view_identities_associated_with_tier),
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
