import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';

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
                    'Tiers',
                    style: TextStyle(color: Theme.of(context).colorScheme.onPrimary, fontSize: 30),
                  ),
                  Text(
                    'A list of all Tiers',
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
                  IconButton(
                    icon: const Icon(Icons.add),
                    color: Theme.of(context).colorScheme.onPrimary,
                    style: ButtonStyle(
                      backgroundColor: MaterialStateProperty.all<Color>(Theme.of(context).colorScheme.primary),
                      foregroundColor: MaterialStateProperty.all<Color>(Theme.of(context).colorScheme.onPrimary),
                    ),
                    onPressed: () {
                      _showAddTierDialog(context: context);
                    },
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
                elevation: 1,
                child: SizedBox(
                  width: _boxWidth,
                  child: DataTable2(
                    isVerticalScrollBarVisible: true,
                    showCheckboxColumn: false,
                    columns: const [
                      DataColumn2(label: Text('Name'), size: ColumnSize.L),
                      DataColumn2(label: Text('Number of Identities'), size: ColumnSize.L),
                    ],
                    rows: _tiers!
                        .map(
                          (tier) => DataRow2(
                            onLongPress: () {},
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
