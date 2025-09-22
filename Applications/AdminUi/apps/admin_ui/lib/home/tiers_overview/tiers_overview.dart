import 'dart:async';

import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:enmeshed_ui_kit/enmeshed_ui_kit.dart';
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
  List<TierOverview> _tiers = [];

  @override
  void initState() {
    super.initState();

    unawaited(_reloadTiers());
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text(context.l10n.tiersOverview_title)),
      body: Card(
        child: Padding(
          padding: const EdgeInsets.all(8),
          child: Column(
            children: [
              Row(
                crossAxisAlignment: CrossAxisAlignment.end,
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  if (kIsDesktop) IconButton(icon: const Icon(Icons.refresh), onPressed: _reloadTiers, tooltip: context.l10n.reload),
                  Gaps.w8,
                  IconButton.filled(
                    icon: const Icon(Icons.add),
                    color: Theme.of(context).colorScheme.onPrimary,
                    onPressed: () => _showAddTierDialog(context: context),
                  ),
                ],
              ),
              Expanded(
                child: DataTable2(
                  isVerticalScrollBarVisible: true,
                  showCheckboxColumn: false,
                  empty: Text(context.l10n.tiersOverview_noTiersFound),
                  columns: [
                    DataColumn2(label: Text(context.l10n.id), size: ColumnSize.L),
                    DataColumn2(label: Text(context.l10n.name), size: ColumnSize.L),
                    DataColumn2(label: Text(context.l10n.numberOfIdentities), size: ColumnSize.L),
                  ],
                  rows: _tiers
                      .map(
                        (tier) => DataRow2(
                          onTap: () => context.go('/tiers/${tier.id}'),
                          cells: [DataCell(Text(tier.id)), DataCell(Text(tier.name)), DataCell(Text('${tier.numberOfIdentities}'))],
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

  Future<void> _showAddTierDialog({required BuildContext context}) async {
    final tier = await showCreateTierDialog(context: context);
    if (tier == null) return;

    await _reloadTiers();
  }

  Future<void> _reloadTiers() async {
    final response = await GetIt.I.get<AdminApiClient>().tiers.getTiers();
    final tiers = response.data..sort((a, b) => a.name.compareTo(b.name));

    setState(() {
      _tiers = tiers;
    });
  }
}
