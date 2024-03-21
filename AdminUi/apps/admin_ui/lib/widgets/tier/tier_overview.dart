import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';

class TierOverview extends StatefulWidget {
  const TierOverview({super.key});

  @override
  State<TierOverview> createState() => _TierOverviewState();
}

class _TierOverviewState extends State<TierOverview> {
  List<Tier> tiers = [];

  @override
  void initState() {
    super.initState();
    loadTiers();
  }

  Future<void> loadTiers() async {
    final response = await GetIt.I.get<AdminApiClient>().tiers.getTiers();
    setState(() {
      tiers = response.data;
    });
  }

  void _handleRowTap(Tier tier) {
    // TODO(stamenione): handle the navigation to tier detail page
    print('Clicked on tier ${tier.id}');
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Tiers'),
        centerTitle: true,
        bottom: const PreferredSize(
          preferredSize: Size.fromHeight(20),
          child: Text(
            'Subtitle Here',
            style: TextStyle(fontSize: 14),
          ),
        ),
      ),
      body: SingleChildScrollView(
        child: Center(
          child: DataTable(
            columns: const <DataColumn>[
              DataColumn(label: Center(child: Text('ID'))),
              DataColumn(label: Center(child: Text('Name'))),
            ],
            rows: tiers.map((tier) {
              return DataRow(
                cells: [
                  DataCell(Text(tier.id)),
                  DataCell(Text(tier.name)),
                ],
                onLongPress: () {
                  _handleRowTap(tier);
                },
              );
            }).toList(),
          ),
        ),
      ),
    );
  }
}
