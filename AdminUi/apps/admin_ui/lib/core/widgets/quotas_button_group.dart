import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';

import '../constants.dart';
import '../modals/modals.dart';

class QuotasButtonGroup extends StatefulWidget {
  final List<String> selectedQuotas;
  final VoidCallback onQuotasChanged;
  final String? identityAddress;
  final String? tierId;

  const QuotasButtonGroup({
    required this.selectedQuotas,
    required this.onQuotasChanged,
    this.identityAddress,
    this.tierId,
    super.key,
  });

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
        children: [
          IconButton(
            icon: Icon(
              Icons.delete,
              color: widget.selectedQuotas.isNotEmpty ? Theme.of(context).colorScheme.onError : null,
            ),
            style: ButtonStyle(
              backgroundColor: WidgetStateProperty.resolveWith((states) {
                return widget.selectedQuotas.isNotEmpty ? Theme.of(context).colorScheme.error : null;
              }),
            ),
            onPressed: widget.selectedQuotas.isNotEmpty ? _removeSelectedQuotas : null,
          ),
          Gaps.w8,
          IconButton.filled(
            icon: const Icon(Icons.add),
            onPressed: () => showAddQuotaDialog(
              context: context,
              address: widget.identityAddress,
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
      title: 'Remove Quotas',
      message:
          'Are you sure you want to remove the selected quotas from ${widget.identityAddress != null ? 'the identity "${widget.identityAddress}"' : 'the tier "${widget.tierId}"'}?',
    );

    if (!confirmed) return;

    for (final quota in widget.selectedQuotas) {
      final result = await _deleteQuota(quota);
      if (result.hasError && mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('An error occurred while deleting the quota(s). Please try again.'),
            showCloseIcon: true,
          ),
        );

        return;
      }
    }

    widget.onQuotasChanged();
    widget.selectedQuotas.clear();
    if (mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Selected quota(s) have been removed.'),
          showCloseIcon: true,
        ),
      );
    }
  }

  Future<ApiResponse<void>> _deleteQuota(String quota) {
    final client = GetIt.I.get<AdminApiClient>();

    if (widget.identityAddress != null) return client.quotas.deleteIdentityQuota(address: widget.identityAddress!, individualQuotaId: quota);
    return client.quotas.deleteTierQuota(tierId: widget.tierId!, tierQuotaDefinitionId: quota);
  }
}
