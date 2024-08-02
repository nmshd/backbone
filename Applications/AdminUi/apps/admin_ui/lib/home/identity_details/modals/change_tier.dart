import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';

import '/core/core.dart';

Future<void> showChangeTierDialog({
  required BuildContext context,
  required VoidCallback onTierUpdated,
  required Identity identityDetails,
  required List<TierOverviewResponse> availableTiers,
}) async {
  await showDialog<void>(
    context: context,
    builder: (BuildContext context) => _ShowChangeTierDialog(
      onTierUpdated: onTierUpdated,
      identityDetails: identityDetails,
      availableTiers: availableTiers,
    ),
  );
}

class _ShowChangeTierDialog extends StatefulWidget {
  final VoidCallback onTierUpdated;
  final Identity identityDetails;
  final List<TierOverviewResponse> availableTiers;

  const _ShowChangeTierDialog({
    required this.onTierUpdated,
    required this.identityDetails,
    required this.availableTiers,
  });

  @override
  State<_ShowChangeTierDialog> createState() => _ShowChangeTierDialogState();
}

class _ShowChangeTierDialogState extends State<_ShowChangeTierDialog> {
  bool _saving = false;
  late String selectedTier;

  @override
  void initState() {
    super.initState();

    selectedTier = widget.identityDetails.tierId;
  }

  @override
  Widget build(BuildContext context) {
    return PopScope(
      canPop: !_saving,
      child: AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
        title: Text(context.l10n.changeTier, textAlign: TextAlign.center),
        contentPadding: const EdgeInsets.only(left: 24, right: 24, top: 20, bottom: 32),
        content: SizedBox(
          width: 500,
          child: DropdownButtonFormField<String>(
            value: selectedTier,
            decoration: const InputDecoration(border: OutlineInputBorder()),
            onChanged: _saving ? null : (String? newValue) => setState(() => selectedTier = newValue!),
            items: widget.availableTiers.where((tier) => tier.canBeManuallyAssigned).map((TierOverviewResponse tier) {
              return DropdownMenuItem<String>(
                value: tier.id,
                child: Text(tier.name),
              );
            }).toList(),
          ),
        ),
        actions: [
          OutlinedButton(
            onPressed: _saving ? null : () => Navigator.of(context, rootNavigator: true).pop(),
            child: Text(context.l10n.cancel),
          ),
          FilledButton(
            onPressed: _saving || selectedTier == widget.identityDetails.tierId ? null : _changeTier,
            child: Text(context.l10n.save),
          ),
        ],
      ),
    );
  }

  Future<void> _changeTier() async {
    setState(() => _saving = true);

    final response = await GetIt.I.get<AdminApiClient>().identities.updateIdentity(widget.identityDetails.address, tierId: selectedTier);

    if (!mounted) return;

    if (response.hasError) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(context.l10n.changeTierDialog_error),
          duration: const Duration(seconds: 3),
        ),
      );

      setState(() => _saving = false);

      return;
    }

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(context.l10n.changeTierDialog_success),
        duration: const Duration(seconds: 3),
      ),
    );

    widget.onTierUpdated();

    Navigator.of(context, rootNavigator: true).pop();
  }
}
