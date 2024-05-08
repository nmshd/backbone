import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';

import '../identities_overview/identities_data_table_source.dart';
import '../identities_overview/identities_filter.dart';
import '/core/core.dart';
import 'modals/modals.dart';

class TiersDetail extends StatefulWidget {
  final String tierId;

  const TiersDetail({required this.tierId, super.key});

  @override
  State<TiersDetail> createState() => _TiersDetailState();
}

class _TiersDetailState extends State<TiersDetail> {
  TierDetails? _tierDetails;

  @override
  void initState() {
    super.initState();

    _reload();
  }

  @override
  Widget build(BuildContext context) {
    if (_tierDetails == null) return const Center(child: CircularProgressIndicator());

    final tierDetails = _tierDetails!;
    return Scrollbar(
      child: SingleChildScrollView(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                const BackButton(),
                Card(
                  child: Padding(
                    padding: const EdgeInsets.all(16),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text.rich(
                          TextSpan(
                            children: [
                              TextSpan(text: 'Tier ID: ', style: Theme.of(context).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.bold)),
                              TextSpan(text: tierDetails.id, style: Theme.of(context).textTheme.bodyLarge),
                            ],
                          ),
                        ),
                        Text.rich(
                          TextSpan(
                            children: [
                              TextSpan(text: 'Tier Name: ', style: Theme.of(context).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.bold)),
                              TextSpan(text: tierDetails.name, style: Theme.of(context).textTheme.bodyLarge),
                            ],
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              ],
            ),
            Gaps.h8,
            _QuotaList(tierDetails, _reload),
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

  @override
  Widget build(BuildContext context) {
    return ExpansionTile(
      initiallyExpanded: true,
      title: const Text('Quotas'),
      subtitle: const Text('View and assign quotas for this tier.'),
      children: [
        if (widget.tierDetails.id != 'TIR00000000000000001')
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
                    backgroundColor: MaterialStateProperty.resolveWith((states) {
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
          child: DataTable(
            columns: const [
              DataColumn(label: Text('Metric')),
              DataColumn(label: Text('Max')),
              DataColumn(label: Text('Period')),
            ],
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
      await GetIt.I.get<AdminApiClient>().quotas.deleteTierQuota(tierId: widget.tierDetails.id, tierQuotaDefinitionId: quota);
      widget.onQuotasChanged();
    }

    _selectedQuotas.clear();
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

  int _sortColumnIndex = 0;

  bool _sortColumnAscending = true;

  int _rowsPerPage = 5;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();

    _dataSource = IdentityDataTableSource(locale: Localizations.localeOf(context));
  }

  @override
  void dispose() {
    _dataSource.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return ExpansionTile(
      initiallyExpanded: true,
      title: const Text('Identities'),
      subtitle: const Text('View Identities associated with this Tier.'),
      children: [
        IdentitiesFilter(
          fixedTierId: widget.tierDetails.id,
          onFilterChanged: ({IdentityOverviewFilter? filter}) async {
            _dataSource
              ..filter = filter
              ..refreshDatasource();
          },
        ),
        SizedBox(
          height: 500,
          child: AsyncPaginatedDataTable2(
            rowsPerPage: _rowsPerPage,
            onRowsPerPageChanged: _setRowsPerPage,
            sortColumnIndex: _sortColumnIndex,
            sortAscending: _sortColumnAscending,
            showFirstLastButtons: true,
            columnSpacing: 5,
            source: _dataSource,
            isVerticalScrollBarVisible: true,
            renderEmptyRowsInTheEnd: false,
            availableRowsPerPage: const [5, 10, 25, 50, 100],
            errorBuilder: (error) => Center(
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  const Text('An error occurred loading the data.'),
                  Gaps.h16,
                  FilledButton(onPressed: _dataSource.refreshDatasource, child: const Text('Retry')),
                ],
              ),
            ),
            columns: <DataColumn2>[
              DataColumn2(label: const Text('Address'), size: ColumnSize.L, onSort: _sort),
              const DataColumn2(label: Text('Tier'), size: ColumnSize.S),
              DataColumn2(label: const Text('Created with Client'), onSort: _sort),
              DataColumn2(label: const Text('Number of Devices'), onSort: _sort),
              DataColumn2(label: const Text('Created at'), size: ColumnSize.S, onSort: _sort),
              DataColumn2(label: const Text('Last Login at'), size: ColumnSize.S, onSort: _sort),
              DataColumn2(label: const Text('Datawallet version'), onSort: _sort),
              DataColumn2(label: const Text('Identity Version'), onSort: _sort),
            ],
          ),
        ),
      ],
    );
  }

  void _setRowsPerPage(int? newValue) {
    _rowsPerPage = newValue ?? _rowsPerPage;
    _dataSource.refreshDatasource();
  }

  void _sort(int columnIndex, bool ascending) {
    setState(() {
      _sortColumnIndex = columnIndex;
      _sortColumnAscending = ascending;
    });
    _dataSource
      ..sort(sortColumnIndex: _sortColumnIndex, sortColumnAscending: _sortColumnAscending)
      ..refreshDatasource();
  }
}
