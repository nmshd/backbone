import 'dart:io';

import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:intl/intl.dart';

import '/core/core.dart';
import 'identity_quotas_table/identity_quotas_table.dart';
import 'modals/change_tier.dart';

class IdentityDetails extends StatefulWidget {
  final String address;

  const IdentityDetails({required this.address, super.key});

  @override
  State<IdentityDetails> createState() => _IdentityDetailsState();
}

class _IdentityDetailsState extends State<IdentityDetails> {
  Identity? _identityDetails;
  List<TierOverview>? _tiers;
  String? _selectedTier;

  late final ScrollController _scrollController;

  @override
  void initState() {
    super.initState();

    _scrollController = ScrollController();

    _reloadIdentity();
    _reloadTiers();
  }

  @override
  void dispose() {
    _scrollController.dispose();

    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    if (_identityDetails == null || _tiers == null) return const Center(child: CircularProgressIndicator());

    final identityDetails = _identityDetails!;
    return Scrollbar(
      controller: _scrollController,
      child: SingleChildScrollView(
        controller: _scrollController,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (Platform.isMacOS || Platform.isWindows) const BackButton(),
            _IdentityDetailsCard(
              identityDetails: identityDetails,
              selectedTier: _selectedTier,
              onTierChanged: (String? newValue) {
                setState(() {
                  _selectedTier = newValue;
                });
              },
              availableTiers: _tiers!,
              updateTierOfIdentity: _reloadIdentity,
            ),
            Gaps.h16,
            IdentityQuotaList(identityDetails, _reloadIdentity),
          ],
        ),
      ),
    );
  }

  Future<void> _reloadIdentity() async {
    final identityDetails = await GetIt.I.get<AdminApiClient>().identities.getIdentity(widget.address);
    if (mounted) {
      setState(() {
        _identityDetails = identityDetails.data;
        _selectedTier = _identityDetails!.tierId;
      });
    }
  }

  Future<void> _reloadTiers() async {
    final tiers = await GetIt.I.get<AdminApiClient>().tiers.getTiers();
    if (mounted) {
      setState(() => _tiers = tiers.data);
    }
  }
}

class _IdentityDetailsCard extends StatelessWidget {
  final Identity identityDetails;
  final String? selectedTier;
  final ValueChanged<String?>? onTierChanged;
  final List<TierOverview> availableTiers;
  final VoidCallback updateTierOfIdentity;

  const _IdentityDetailsCard({
    required this.identityDetails,
    required this.availableTiers,
    required this.updateTierOfIdentity,
    this.selectedTier,
    this.onTierChanged,
  });

  @override
  Widget build(BuildContext context) {
    final currentTier = availableTiers.firstWhere((tier) => tier.id == identityDetails.tierId);

    return Row(
      children: [
        Expanded(
          child: Card(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      Text(identityDetails.address, style: Theme.of(context).textTheme.headlineLarge),
                      Gaps.w16,
                      CopyToClipboardButton(
                        clipboardText: identityDetails.address,
                        successMessage: context.l10n.identity_copied_to_clipboard_message,
                      ),
                    ],
                  ),
                  const SizedBox(height: 32),
                  Wrap(
                    crossAxisAlignment: WrapCrossAlignment.center,
                    spacing: 8,
                    runSpacing: 8,
                    children: [
                      _IdentityDetails(
                        title: context.l10n.client_ID,
                        value: identityDetails.clientId,
                      ),
                      _IdentityDetails(
                        title: context.l10n.public_key,
                        value: identityDetails.publicKey.ellipsize(20),
                        onIconPressed: () => context.setClipboardDataWithSuccessNotification(
                          clipboardText: identityDetails.publicKey,
                          successMessage: context.l10n.public_key_copied_to_clipboard_message,
                        ),
                        icon: Icons.copy,
                        tooltipMessage: context.l10n.copy_public_key,
                      ),
                      _IdentityDetails(
                        title: context.l10n.createdAt,
                        value:
                            '${DateFormat.yMd(Localizations.localeOf(context).languageCode).format(identityDetails.createdAt)} ${DateFormat.Hms().format(identityDetails.createdAt)}',
                      ),
                      _IdentityDetails(
                        title: context.l10n.tier,
                        value: currentTier.name,
                        onIconPressed: currentTier.canBeManuallyAssigned
                            ? () => showChangeTierDialog(
                                  context: context,
                                  onTierUpdated: updateTierOfIdentity,
                                  identityDetails: identityDetails,
                                  availableTiers: availableTiers,
                                )
                            : null,
                        icon: Icons.edit,
                        tooltipMessage: context.l10n.change_tier,
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

class _IdentityDetails extends StatelessWidget {
  final String title;
  final String value;
  final VoidCallback? onIconPressed;
  final IconData? icon;
  final String? tooltipMessage;

  const _IdentityDetails({
    required this.title,
    required this.value,
    this.onIconPressed,
    this.icon,
    this.tooltipMessage,
  });

  @override
  Widget build(BuildContext context) {
    assert(
      onIconPressed == null || (onIconPressed != null && icon != null || tooltipMessage != null),
      'If edit is provided, icon and tooltipMessage must be provided too.',
    );

    return RawChip(
      label: Text.rich(
        TextSpan(
          children: [
            TextSpan(text: '$title ', style: Theme.of(context).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.bold)),
            TextSpan(text: value, style: Theme.of(context).textTheme.bodyLarge),
          ],
        ),
      ),
      onDeleted: onIconPressed,
      deleteIcon: Icon(icon),
      deleteButtonTooltipMessage: tooltipMessage,
    );
  }
}
