import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '/core/core.dart';

class IdentitiesOverview extends StatefulWidget {
  const IdentitiesOverview({super.key});

  @override
  State<IdentitiesOverview> createState() => _IdentitiesOverviewState();
}

class _IdentitiesOverviewState extends State<IdentitiesOverview> {
  late IdentityDataTableSource _dataSource;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();

    _dataSource = IdentityDataTableSource(
      locale: Localizations.localeOf(context),
      navigateToIdentity: ({required String address}) {
        context.go('/identities/$address');
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
    return Scaffold(
      appBar: AppBar(title: const Text('A list of existing Identities')),
      body: Card(
        child: Padding(
          padding: const EdgeInsets.all(8),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              IdentitiesFilter(
                onFilterChanged: ({IdentityOverviewFilter? filter}) async {
                  _dataSource
                    ..filter = filter
                    ..refreshDatasource();
                },
              ),
              Expanded(child: IdentitiesDataTable(dataSource: _dataSource)),
            ],
          ),
        ),
      ),
    );
  }
}
