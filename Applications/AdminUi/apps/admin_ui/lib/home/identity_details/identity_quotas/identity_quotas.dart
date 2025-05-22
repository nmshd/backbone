import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';

import '/core/core.dart';

class IdentityQuotas extends StatefulWidget {
  final Identity identityDetails;
  final VoidCallback onQuotasChanged;

  const IdentityQuotas(this.identityDetails, this.onQuotasChanged, {super.key});

  @override
  State<IdentityQuotas> createState() => IdentityQuotasState();
}

class IdentityQuotasState extends State<IdentityQuotas> {
  final List<String> _selectedQuotas = [];

  bool get isQueuedForDeletionTier => widget.identityDetails.tierId == 'TIR00000000000000001';

  @override
  Widget build(BuildContext context) {
    final groupedQuotas = _groupQuotas();

    return Theme(
      data: Theme.of(context).copyWith(dividerColor: Colors.transparent),
      child: ExpansionTile(
        title: Text(context.l10n.quotas),
        subtitle: Text(context.l10n.identityQuotaTable_title_description),
        children: [
          Card(
            child: Column(
              children: [
                if (!isQueuedForDeletionTier)
                  QuotasButtonGroup(
                    selectedQuotas: _selectedQuotas,
                    identityAddress: widget.identityDetails.address,
                    onQuotasChanged: widget.onQuotasChanged,
                  ),
                SizedBox(
                  width: double.infinity,
                  height: 500,
                  child: DataTable2(
                    columns: [
                      DataColumn2(label: Text(context.l10n.metric)),
                      DataColumn2(label: Text(context.l10n.source), size: ColumnSize.S),
                      DataColumn2(label: Text('${context.l10n.usage} (${context.l10n.used}/${context.l10n.max})'), size: ColumnSize.L),
                      DataColumn2(label: Text(context.l10n.period), size: ColumnSize.S),
                      const DataColumn2(label: Text(''), size: ColumnSize.S),
                    ],
                    empty: Text(context.l10n.identityQuotaTable_noQuotaApplied),
                    rows: groupedQuotas.entries.expand((entry) {
                      final metricName = entry.key;
                      final quotas = entry.value;

                      final hasIndividualQuota = quotas.any((quota) => quota.source == context.l10n.identityQuotaTable_individual);

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
                        ...quotas.map((quota) {
                          final isTierQuota = quota.source == 'Tier';
                          final shouldDisable = isTierQuota && hasIndividualQuota;
                          final tooltipMessage = shouldDisable ? context.l10n.identityQuotaTable_tierQuotaEffectMessage : null;

                          return DataRow2(
                            selected: _selectedQuotas.contains(quota.id),
                            color: shouldDisable ? WidgetStateProperty.all(Theme.of(context).colorScheme.surfaceBright) : null,
                            onSelectChanged: shouldDisable || isTierQuota ? null : (_) => _toggleSelection(quota.id),
                            cells: [
                              DataCell(Container()),
                              DataCell(Text(quota.source, style: TextStyle(color: shouldDisable ? Colors.grey : null))),
                              DataCell(
                                Row(
                                  children: [
                                    Text('${quota.usage}/${quota.max}', style: TextStyle(color: shouldDisable ? Colors.grey : null)),
                                    const SizedBox(width: 8),
                                    Expanded(
                                      child: LinearProgressIndicator(
                                        value: quota.max > 0 ? quota.usage / quota.max : 0,
                                        backgroundColor: shouldDisable ? Colors.grey : Theme.of(context).colorScheme.inversePrimary,
                                        valueColor: AlwaysStoppedAnimation<Color>(
                                          shouldDisable ? Colors.grey : Theme.of(context).colorScheme.primary,
                                        ),
                                        minHeight: 8,
                                      ),
                                    ),
                                  ],
                                ),
                              ),
                              DataCell(Text(quota.period, style: TextStyle(color: shouldDisable ? Colors.grey : null))),
                              DataCell(
                                Tooltip(
                                  message: tooltipMessage ?? '',
                                  child: isTierQuota && shouldDisable ? Icon(Icons.info, color: shouldDisable ? Colors.grey : null) : null,
                                ),
                              ),
                            ],
                          );
                        }),
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

    for (final quota in widget.identityDetails.quotas) {
      if (groupedQuotas.containsKey(quota.metric.displayName)) {
        groupedQuotas[quota.metric.displayName]!.add(quota);
      } else {
        groupedQuotas[quota.metric.displayName] = [quota];
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
}
