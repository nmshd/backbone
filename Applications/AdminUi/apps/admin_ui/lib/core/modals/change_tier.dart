import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';

import '/core/core.dart';

Future<void> showChangeTierDialog({
  required BuildContext context,
  required VoidCallback onTierUpdated,
  required List<TierOverview> availableTiers,
  Identity? identityDetails,
  Client? clientDetails,
}) async {
  assert(identityDetails != null || clientDetails != null, 'Either identity details or client details must be provided');
  assert(identityDetails == null || clientDetails == null, 'Only one of identity details or client details can be provided');

  await showDialog<void>(
    context: context,
    builder: (BuildContext context) => _ShowChangeTierDialog(
      onTierUpdated: onTierUpdated,
      currentTier: (clientDetails?.defaultTier ?? identityDetails?.tierId)!,
      assignTier: ({required String tierId}) {
        if (clientDetails != null) {
          return GetIt.I.get<AdminApiClient>().clients.updateClient(
            clientDetails.clientId,
            defaultTier: tierId,
            maxIdentities: clientDetails.maxIdentities,
          );
        }

        return GetIt.I.get<AdminApiClient>().identities.updateIdentity(identityDetails!.address, tierId: tierId);
      },
      availableTiers: availableTiers,
    ),
  );
}

class _ShowChangeTierDialog extends StatefulWidget {
  final VoidCallback onTierUpdated;
  final Future<ApiResponse<dynamic>> Function({required String tierId}) assignTier;
  final List<TierOverview> availableTiers;
  final String currentTier;

  const _ShowChangeTierDialog({required this.onTierUpdated, required this.availableTiers, required this.assignTier, required this.currentTier});

  @override
  State<_ShowChangeTierDialog> createState() => _ShowChangeTierDialogState();
}

class _ShowChangeTierDialogState extends State<_ShowChangeTierDialog> {
  bool _saving = false;
  late String _selectedTier;

  @override
  void initState() {
    super.initState();

    _selectedTier = widget.currentTier;
  }

  @override
  Widget build(BuildContext context) {
    return PopScope(
      canPop: !_saving,
      child: AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: .circular(8)),
        title: Text(context.l10n.changeTier, textAlign: .center),
        contentPadding: const EdgeInsets.only(left: 24, right: 24, top: 20, bottom: 32),
        content: SizedBox(
          width: 500,
          child: DropdownButtonFormField<String>(
            initialValue: _selectedTier,
            decoration: const InputDecoration(border: OutlineInputBorder()),
            onChanged: _saving ? null : (String? newValue) => setState(() => _selectedTier = newValue!),
            items: widget.availableTiers.where((tier) => tier.canBeManuallyAssigned || tier.canBeUsedAsDefaultForClient).map((TierOverview tier) {
              return DropdownMenuItem<String>(value: tier.id, child: Text(tier.name));
            }).toList(),
          ),
        ),
        actions: [
          OutlinedButton(onPressed: _saving ? null : () => Navigator.of(context, rootNavigator: true).pop(), child: Text(context.l10n.cancel)),
          FilledButton(onPressed: _saving || _selectedTier == widget.currentTier ? null : _changeTier, child: Text(context.l10n.change)),
        ],
      ),
    );
  }

  Future<void> _changeTier() async {
    setState(() => _saving = true);

    final response = await widget.assignTier(tierId: _selectedTier);

    if (!mounted) return;

    if (response.hasError) {
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(context.l10n.changeTierDialog_error), duration: const Duration(seconds: 3)));

      setState(() => _saving = false);

      return;
    }

    ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(context.l10n.changeTierDialog_success), duration: const Duration(seconds: 3)));

    widget.onTierUpdated();

    Navigator.of(context, rootNavigator: true).pop();
  }
}
