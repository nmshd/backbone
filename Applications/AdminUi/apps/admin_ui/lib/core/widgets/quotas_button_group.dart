import 'dart:async';

import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:sn_progress_dialog/enums/progress_types.dart';
import 'package:sn_progress_dialog/progress_dialog.dart';

import '../extensions.dart';
import '../modals/modals.dart';

class QuotasButtonGroup extends StatefulWidget {
  final List<String> selectedQuotas;
  final VoidCallback onQuotasChanged;
  final String? identityAddress;
  final String? tierId;

  const QuotasButtonGroup({required this.selectedQuotas, required this.onQuotasChanged, this.identityAddress, this.tierId, super.key})
    : assert(identityAddress != null || tierId != null, 'Either identityAddress or tierId must be provided'),
      assert(identityAddress == null || tierId == null, 'Only one of identityAddress or tierId can be provided');

  @override
  State<QuotasButtonGroup> createState() => _QuotasButtonGroupState();
}

class _QuotasButtonGroupState extends State<QuotasButtonGroup> {
  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.end,
        spacing: 8,
        children: [
          IconButton(
            icon: Icon(Icons.delete, color: widget.selectedQuotas.isNotEmpty ? Theme.of(context).colorScheme.onError : null),
            style: ButtonStyle(
              backgroundColor: WidgetStateProperty.resolveWith((states) {
                return widget.selectedQuotas.isNotEmpty ? Theme.of(context).colorScheme.error : null;
              }),
            ),
            onPressed: widget.selectedQuotas.isNotEmpty ? _removeSelectedQuotas : null,
          ),
          IconButton.filled(
            icon: const Icon(Icons.add),
            onPressed: () => showAddQuotaDialog(
              context: context,
              identityAddress: widget.identityAddress,
              tierId: widget.tierId,
              onQuotaAdded: widget.onQuotasChanged,
            ),
          ),
        ],
      ),
    );
  }

  Future<void> _removeSelectedQuotas() async {
    final confirmed = await showConfirmationDialog(
      context: context,
      title: context.l10n.quotaButtonGroup_removeQuotas_title,
      message:
          '${context.l10n.quotaButtonGroup_deletionMessage} ${widget.identityAddress != null ? '${context.l10n.quotaButtonGroup_theIdentity} "${widget.identityAddress}"' : '${context.l10n.quotaButtonGroup_theTier} "${widget.tierId}"'}?',
      actionText: context.l10n.remove,
    );

    if (!confirmed) return;

    ProgressDialog? progressDialog;

    if (mounted) {
      progressDialog = ProgressDialog(context: context);

      unawaited(
        progressDialog.show(
          max: widget.selectedQuotas.length,
          msg: context.l10n.quotaButtonGroup_deletingMessage,
          progressType: ProgressType.determinate,
        ),
      );
    }

    for (final quota in widget.selectedQuotas.indexed) {
      final result = await _deleteQuota(quota.$2);
      if (result.hasError && mounted) {
        ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(context.l10n.quotaButtonGroup_errorDeletingQuota), showCloseIcon: true));

        return;
      }

      progressDialog?.update(value: quota.$1 + 1);
    }

    progressDialog?.close();

    widget.onQuotasChanged();
    widget.selectedQuotas.clear();
    if (mounted) {
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(context.l10n.quotaButtonGroup_selectedQuotaRemoved), showCloseIcon: true));
    }
  }

  Future<ApiResponse<void>> _deleteQuota(String quota) {
    final client = GetIt.I.get<AdminApiClient>();

    if (widget.identityAddress != null) return client.identities.deleteIndividualQuota(address: widget.identityAddress!, individualQuotaId: quota);
    return client.quotas.deleteTierQuota(tierId: widget.tierId!, tierQuotaDefinitionId: quota);
  }
}
