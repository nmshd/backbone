import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';

import '/core/core.dart';
import 'modals/show_create_tier_dialog.dart';

class TiersOverview extends StatefulWidget {
  const TiersOverview({super.key});

  @override
  State<TiersOverview> createState() => _TiersOverviewState();
}

class _TiersOverviewState extends State<TiersOverview> {
  List<TierOverview>? _tiers;

  final double _boxWidth = 700;

  @override
  void initState() {
    super.initState();

    _reloadTiers();
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(top: 20),
      child: Column(
        children: [
          Container(
            height: 100,
            width: _boxWidth,
            decoration: BoxDecoration(
              color: Theme.of(context).colorScheme.primary,
              boxShadow: const [
                BoxShadow(
                  color: Colors.black26,
                  blurRadius: 4,
                ),
              ],
            ),
            child: Padding(
              padding: const EdgeInsets.only(left: 15, top: 20),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    context.l10n.tiers,
                    style: TextStyle(color: Theme.of(context).colorScheme.onPrimary, fontSize: 30),
                  ),
                  Text(
                    context.l10n.tiersOverview_title_description,
                    style: TextStyle(color: Theme.of(context).colorScheme.onPrimary, fontSize: 13),
                  ),
                ],
              ),
            ),
          ),
          Padding(
            padding: const EdgeInsets.symmetric(vertical: 8),
            child: SizedBox(
              width: _boxWidth,
              child: Row(
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  IconButton.filled(
                    icon: const Icon(Icons.add),
                    color: Theme.of(context).colorScheme.onPrimary,
                    onPressed: () => _showAddTierDialog(context: context),
                  ),
                ],
              ),
            ),
          ),
          if (_tiers == null)
            const Center(child: CircularProgressIndicator())
          else
            Expanded(
              child: Card(
                child: SizedBox(
                  width: _boxWidth,
                  child: DataTable2(
                    isVerticalScrollBarVisible: true,
                    showCheckboxColumn: false,
                    empty: Text(context.l10n.tiersOverview_noTiersFound),
                    columns: [
                      DataColumn2(label: Text(context.l10n.name), size: ColumnSize.L),
                      DataColumn2(label: Text(context.l10n.numberOfIdentities), size: ColumnSize.L),
                    ],
                    rows: _tiers!
                        .map(
                          (tier) => DataRow2(
                            onTap: () => context.go('/tiers/${tier.id}'),
                            cells: [
                              DataCell(Text(tier.name)),
                              DataCell(Text('${tier.numberOfIdentities}')),
                            ],
                          ),
                        )
                        .toList(),
                  ),
                ),
              ),
            ),
        ],
      ),
    );
  }

  Future<void> _showAddTierDialog({required BuildContext context}) async {
    final tier = await showCreateTierDialog(context: context);
    if (tier == null) return;

    await _reloadTiers();
  }

  Future<void> _reloadTiers() async {
    final response = await GetIt.I.get<AdminApiClient>().tiers.getTiers();

    setState(() {
      _tiers = response.data;
    });
  }
}
