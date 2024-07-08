import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '/core/core.dart';
import 'deletion_process_audit_logs/deletion_process_auditlogs.dart';

class IdentitiesOverview extends StatefulWidget {
  const IdentitiesOverview({super.key});

  @override
  State<IdentitiesOverview> createState() => _IdentitiesOverviewState();
}

class _IdentitiesOverviewState extends State<IdentitiesOverview> {
  late IdentityDataTableSource _dataSource;
  late final ScrollController _scrollController;

  @override
  void initState() {
    super.initState();

    _scrollController = ScrollController();
  }

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
      body: Scrollbar(
        controller: _scrollController,
        child: SingleChildScrollView(
          controller: _scrollController,
          child: Column(
            children: [
              Card(
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
                      SizedBox(
                        height: 700,
                        child: IdentitiesDataTable(dataSource: _dataSource),
                      ),
                    ],
                  ),
                ),
              ),
              Gaps.h8,
              const DeletionProcessAuditLogs(),
            ],
          ),
        ),
      ),
    );
  }
}
