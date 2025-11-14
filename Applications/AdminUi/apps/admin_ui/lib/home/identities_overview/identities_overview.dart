import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '/core/core.dart';
import 'widgets/query_deletion_process_audit_logs.dart';

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
      appBar: AppBar(title: Text(context.l10n.identityOverview_title)),
      body: Column(
        children: [
          Expanded(
            child: Card(
              child: Padding(
                padding: const EdgeInsets.all(8),
                child: Column(
                  crossAxisAlignment: .end,
                  mainAxisSize: .min,
                  children: [
                    IdentitiesFilter(
                      onFilterChanged: ({IdentityOverviewFilter? filter}) async {
                        _dataSource
                          ..filter = filter
                          ..refreshDatasource();
                      },
                    ),
                    IconButton(icon: const Icon(Icons.refresh), onPressed: () async => _dataSource.refreshDatasource(), tooltip: context.l10n.reload),
                    Expanded(child: IdentitiesDataTable(dataSource: _dataSource)),
                  ],
                ),
              ),
            ),
          ),
          Gaps.h8,
          const QueryDeletionProcessAuditLogs(),
        ],
      ),
    );
  }
}
