import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';

import '/core/core.dart';

class DeletionProcessAuditLogsDetails extends StatefulWidget {
  final String identityAddress;

  const DeletionProcessAuditLogsDetails({
    required this.identityAddress,
    super.key,
  });

  @override
  State<DeletionProcessAuditLogsDetails> createState() => _DeletionProcessAuditLogsDetailsState();
}

class _DeletionProcessAuditLogsDetailsState extends State<DeletionProcessAuditLogsDetails> {
  List<IdentityDeletionProcessAuditLogEntry>? _identityDeletionProcessAuditLogs;

  @override
  void initState() {
    super.initState();
    _loadIdentityDeletionProcessAuditLogs();
  }

  @override
  Widget build(BuildContext context) {
    if (_identityDeletionProcessAuditLogs == null) {
      return const Center(child: CircularProgressIndicator());
    }

    final identityDeletionProcessAuditLogs = _identityDeletionProcessAuditLogs!;
    return Column(
      mainAxisSize: MainAxisSize.min,
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        if (kIsDesktop) const BackButton(),
        Row(
          children: [
            Expanded(
              child: Card(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Padding(
                      padding: const EdgeInsets.all(8),
                      child: EntityDetails(title: context.l10n.identity, value: widget.identityAddress),
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
        Expanded(child: DeletionProcessAuditLogsTable(auditLogs: identityDeletionProcessAuditLogs)),
      ],
    );
  }

  Future<void> _loadIdentityDeletionProcessAuditLogs() async {
    final response = await GetIt.I.get<AdminApiClient>().identities.getIdentityDeletionProcessAuditLogs(address: widget.identityAddress);
    if (mounted) {
      setState(() {
        _identityDeletionProcessAuditLogs = response.data;
      });
    }
  }
}
