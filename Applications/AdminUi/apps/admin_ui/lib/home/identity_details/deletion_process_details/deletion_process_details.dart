import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:enmeshed_ui_kit/enmeshed_ui_kit.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';
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
  IdentityDeletionProcessDetail? _deletionProcessesDetails;

  @override
  void initState() {
    super.initState();

    _reloadIdentityDeletionProcessAuditLogs();
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
          if (kIsDesktop)
            Row(
              children: [
                BackButton(onPressed: () => context.pop(false)),
                IconButton(icon: const Icon(Icons.refresh), onPressed: _reloadIdentityDeletionProcessAuditLogs, tooltip: context.l10n.reload),
              ],
            ),
          _DeletionProcessDetailsCard(address: widget.address, deletionProcessDetails: deletionProcessDetails),
          Gaps.h16,
          Expanded(child: DeletionProcessAuditLogsTable(auditLogs: _deletionProcessesDetails!.auditLog)),
          Gaps.h8,
          Align(
            alignment: Alignment.centerRight,
            child: Padding(
              padding: const EdgeInsets.only(right: 16),
              child: Tooltip(
                message: deletionProcessDetails.status == DeletionProcessStatus.approved
                    ? ''
                    : context.l10n.deletionProcessDetails_cancelDeletionProcess_disabledTooltipMessage,
                child: FilledButton(
                  style: FilledButton.styleFrom(
                    backgroundColor: Theme.of(context).colorScheme.error,
                    textStyle: TextStyle(color: Theme.of(context).colorScheme.onError),
                  ),
                  onPressed: deletionProcessDetails.status != DeletionProcessStatus.approved ? null : _cancelDeletionProcess,
                  child: Text(context.l10n.deletionProcessDetails_cancelDeletionProcess_title),
                ),
              ),
            ),
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

    final result = await GetIt.I.get<AdminApiClient>().identities.cancelDeletionProcess(
      address: widget.address,
      deletionProcessId: widget.deletionProcessId,
    );

    if (result.hasError) {
      if (mounted) {
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(SnackBar(backgroundColor: Theme.of(context).colorScheme.error, content: Text(result.error.message), showCloseIcon: true));
      }
      return;
    }

    if (mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          backgroundColor: Colors.green,
          content: Text(context.l10n.deletionProcessDetails_deletionProcessCancelledSuccessfully),
          showCloseIcon: true,
        ),
      );

      Navigator.of(context).pop(true);
    }
  }

  Future<void> _reloadIdentityDeletionProcessAuditLogs() async {
    final deletionProcessesDetails = await GetIt.I.get<AdminApiClient>().identities.getIdentityDeletionProcess(
      address: widget.address,
      deletionProcessId: widget.deletionProcessId,
    );

    if (!mounted) return;

    if (deletionProcessesDetails.hasError) return context.pushReplacement('/error', extra: deletionProcessesDetails.error.message);

    setState(() => _deletionProcessesDetails = deletionProcessesDetails.data);
  }
}

class _DeletionProcessDetailsCard extends StatelessWidget {
  final String address;
  final IdentityDeletionProcessDetail deletionProcessDetails;

  const _DeletionProcessDetailsCard({required this.address, required this.deletionProcessDetails});

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
                        value: switch (deletionProcessDetails.status) {
                          DeletionProcessStatus.waitingForApproval => context.l10n.deletionProcessDetails_status_waitingForApproval,
                          DeletionProcessStatus.approved => context.l10n.deletionProcessDetails_status_approved,
                          DeletionProcessStatus.cancelled => context.l10n.deletionProcessDetails_status_cancelled,
                          DeletionProcessStatus.rejected => context.l10n.deletionProcessDetails_status_rejected,
                          DeletionProcessStatus.deleting => context.l10n.deletionProcessDetails_status_deleting,
                        },
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
