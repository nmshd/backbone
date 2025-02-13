import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';

import '/core/core.dart';

class DeletionProcessAuditLogDetails extends StatefulWidget {
  final String identityAddress;

  const DeletionProcessAuditLogDetails({required this.identityAddress, super.key});

  @override
  State<DeletionProcessAuditLogDetails> createState() => _DeletionProcessAuditLogDetailsState();
}

class _DeletionProcessAuditLogDetailsState extends State<DeletionProcessAuditLogDetails> {
  List<IdentityDeletionProcessAuditLogEntry>? _identityDeletionProcessAuditLogs;

  @override
  void initState() {
    super.initState();

    _reloadIdentityDeletionProcessAuditLogs();
  }

  @override
  Widget build(BuildContext context) {
    if (_identityDeletionProcessAuditLogs == null) {
      return const Center(child: CircularProgressIndicator());
    }

    final identityDeletionProcessAuditLogs = _identityDeletionProcessAuditLogs!;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        if (kIsDesktop)
          Row(
            children: [
              const BackButton(),
              IconButton(icon: const Icon(Icons.refresh), onPressed: _reloadIdentityDeletionProcessAuditLogs, tooltip: context.l10n.reload),
            ],
          ),
        Card(
          child: Padding(
            padding: const EdgeInsets.all(8),
            child: Align(alignment: Alignment.centerLeft, child: EntityDetails(title: context.l10n.identity, value: widget.identityAddress)),
          ),
        ),
        Expanded(child: DeletionProcessAuditLogsTable(auditLogs: identityDeletionProcessAuditLogs)),
      ],
    );
  }

  Future<void> _reloadIdentityDeletionProcessAuditLogs() async {
    final response = await GetIt.I.get<AdminApiClient>().identities.getIdentityDeletionProcessAuditLogs(address: widget.identityAddress);

    if (!mounted) return;

    if (response.hasError) return context.pushReplacement('/error', extra: response.error.message);

    setState(() => _identityDeletionProcessAuditLogs = response.data);
  }
}
