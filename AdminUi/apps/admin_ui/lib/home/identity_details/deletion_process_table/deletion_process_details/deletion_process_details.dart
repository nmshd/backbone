import 'dart:io';

import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:intl/intl.dart';

import '/core/core.dart';

class DeletionProcessDetails extends StatefulWidget {
  final String address;
  final String deletionProcessId;

  const DeletionProcessDetails({required this.address, required this.deletionProcessId, super.key});

  @override
  State<DeletionProcessDetails> createState() => _DeletionProcessDetailsState();
}

class _DeletionProcessDetailsState extends State<DeletionProcessDetails> {
  DeletionProcessDetail? _deletionProcessesDetails;
  List<AuditLog> auditLogs = [];

  @override
  void initState() {
    super.initState();

    _reloadDeletionProcess();
  }

  @override
  Widget build(BuildContext context) {
    if (_deletionProcessesDetails == null) return const Center(child: CircularProgressIndicator());

    final deletionProcessDetails = _deletionProcessesDetails!;
    return Padding(
      padding: const EdgeInsets.only(bottom: 16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          if (Platform.isMacOS || Platform.isWindows) const BackButton(),
          _DeletionProcessDetailsCard(
            address: widget.address,
            deletionProcessDetails: deletionProcessDetails,
            updateDeletionProcess: _reloadDeletionProcess,
          ),
          Gaps.h16,
          Expanded(child: DeletionProcessAuditLogsTable(auditLogs: auditLogs)),
          Gaps.h16,
          Row(
            mainAxisAlignment: MainAxisAlignment.end,
            children: [
              Padding(
                padding: const EdgeInsets.only(right: 16),
                child: FilledButton(
                  style: ButtonStyle(
                    backgroundColor: WidgetStateProperty.resolveWith((states) {
                      return Theme.of(context).colorScheme.error;
                    }),
                  ),
                  onPressed: _cancelDeletionProcess,
                  child: Text(context.l10n.deletionProcessDetails_cancelDeletionProcess_title),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Future<void> _cancelDeletionProcess() async {
    final confirmed = await showConfirmationDialog(
      actionText: context.l10n.cancel,
      cancelActionText: context.l10n.close,
      context: context,
      message: context.l10n.deletionProcessDetails_cancelDeletionProcess_message,
      title: context.l10n.deletionProcessDetails_cancelDeletionProcess_title,
    );

    if (!confirmed) return;

    final result = await GetIt.I
        .get<AdminApiClient>()
        .deletionProcesses
        .cancelDeletionProcessAsSupport(address: widget.address, deletionProcessId: widget.deletionProcessId);
    if (result.hasError && mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          backgroundColor: Theme.of(context).colorScheme.error,
          content: Text(result.error.message),
          showCloseIcon: true,
        ),
      );

      await _reloadDeletionProcess();
    }
  }

  Future<void> _reloadDeletionProcess() async {
    final deletionProcessesDetails = await GetIt.I
        .get<AdminApiClient>()
        .deletionProcesses
        .getIdentityDeletionProcessDetails(address: widget.address, deletionProcessId: widget.deletionProcessId);
    if (mounted) {
      setState(() {
        _deletionProcessesDetails = deletionProcessesDetails.data;
        auditLogs = deletionProcessesDetails.data.auditLog;
      });
    }
  }
}

class _DeletionProcessDetailsCard extends StatelessWidget {
  final String address;
  final DeletionProcessDetail deletionProcessDetails;
  final VoidCallback updateDeletionProcess;

  const _DeletionProcessDetailsCard({
    required this.address,
    required this.deletionProcessDetails,
    required this.updateDeletionProcess,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Expanded(
          child: Card(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(context.l10n.deletionProcessDetails_title, style: Theme.of(context).textTheme.headlineLarge),
                  Gaps.h32,
                  Wrap(
                    crossAxisAlignment: WrapCrossAlignment.center,
                    spacing: 8,
                    runSpacing: 8,
                    children: [
                      EntityDetails(title: context.l10n.id, value: deletionProcessDetails.id),
                      EntityDetails(title: context.l10n.address, value: address),
                      EntityDetails(
                        title: context.l10n.createdAt,
                        value:
                            '${DateFormat.yMd(Localizations.localeOf(context).languageCode).format(deletionProcessDetails.createdAt)} ${DateFormat.Hms().format(deletionProcessDetails.createdAt)}',
                      ),
                      EntityDetails(
                        title: context.l10n.deletionProcessDetails_status,
                        value: deletionProcessDetails.status == 'WaitingForApproval' ? 'Waiting for Approval' : deletionProcessDetails.status,
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ),
        ),
      ],
    );
  }
}

class _DeletionProcessDetails extends StatelessWidget {
  final String title;
  final String value;

  const _DeletionProcessDetails({required this.title, required this.value});

  @override
  Widget build(BuildContext context) {
    return RawChip(
      label: Text.rich(
        TextSpan(
          children: [
            TextSpan(text: '$title ', style: Theme.of(context).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.bold)),
            TextSpan(text: value, style: Theme.of(context).textTheme.bodyLarge),
          ],
        ),
      ),
    );
  }
}
