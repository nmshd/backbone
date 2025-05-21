import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:enmeshed_ui_kit/enmeshed_ui_kit.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

import '/core/core.dart';
import 'deletion_process_table/deletion_process_table.dart';
import 'identity_devices/identity_devices.dart';
import 'identity_messages/identity_messages.dart';
import 'identity_quotas/identity_quotas.dart';
import 'identity_relationships/identity_relationships.dart';

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
            if (kIsDesktop)
              Row(
                children: [
                  const BackButton(),
                  IconButton(
                    icon: const Icon(Icons.refresh),
                    onPressed: () async {
                      await _reloadIdentity();
                      await _reloadTiers();
                    },
                    tooltip: context.l10n.reload,
                  ),
                ],
              ),
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
            IdentityQuotas(identityDetails, _reloadIdentity),
            Gaps.h16,
            IdentityDevices(devices: identityDetails.devices),
            Gaps.h16,
            IdentityRelationships(address: identityDetails.address),
            Gaps.h16,
            IdentityMessages(
              participant: widget.address,
              type: MessageType.incoming,
              title: context.l10n.identityDetails_receivedMessages_title,
              subtitle: context.l10n.identityDetails_receivedMessages_subtitle,
              emptyTableMessage: context.l10n.identityDetails_noReceivedMessagesFound,
            ),
            Gaps.h16,
            IdentityMessages(
              participant: widget.address,
              type: MessageType.outgoing,
              title: context.l10n.identityDetails_sentMessages_title,
              subtitle: context.l10n.identityDetails_sentMessages_subtitle,
              emptyTableMessage: context.l10n.identityDetails_noSentMessagesFound,
            ),
            Gaps.h16,
            DeletionProcessTable(address: widget.address),
          ],
        ),
      ),
    );
  }

  Future<void> _reloadIdentity() async {
    final identityDetails = await GetIt.I.get<AdminApiClient>().identities.getIdentity(widget.address);

    if (!mounted) return;

    if (identityDetails.hasError) return context.pushReplacement('/error', extra: identityDetails.error.message);

    setState(() {
      _identityDetails = identityDetails.data;
      _selectedTier = _identityDetails!.tierId;
    });
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

    return Card(
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
                  successMessage: context.l10n.identityDetails_card_identityClipboardMessage,
                ),
              ],
            ),
            const SizedBox(height: 32),
            Wrap(
              crossAxisAlignment: WrapCrossAlignment.center,
              spacing: 8,
              runSpacing: 8,
              children: [
                CopyableEntityDetails(title: context.l10n.clientID, value: identityDetails.clientId),
                CopyableEntityDetails(title: context.l10n.identityDetails_card_publicKey, value: identityDetails.publicKey, ellipsize: 20),
                EntityDetails(
                  title: context.l10n.createdAt,
                  value:
                      '${DateFormat.yMd(Localizations.localeOf(context).languageCode).format(identityDetails.createdAt)} ${DateFormat.Hms().format(identityDetails.createdAt)}',
                ),
                EntityDetails(
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
                  tooltipMessage: context.l10n.changeTier,
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
