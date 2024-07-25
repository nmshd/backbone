import 'dart:io';

import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:intl/intl.dart';

import '/core/core.dart';

class ClientDetails extends StatefulWidget {
  final String address;

  const ClientDetails({required this.address, super.key});

  @override
  State<ClientDetails> createState() => _ClientDetailsState();
}

class _ClientDetailsState extends State<ClientDetails> {
  Client? _clientDetails;
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
    if (_clientDetails == null || _tiers == null) return const Center(child: CircularProgressIndicator());

    final clientDetails = _clientDetails!;
    return Scrollbar(
      controller: _scrollController,
      child: SingleChildScrollView(
        controller: _scrollController,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (Platform.isMacOS || Platform.isWindows) const BackButton(),
            _ClientDetailsCard(
              clientDetails: clientDetails,
              selectedTier: _selectedTier,
              onTierChanged: (String? newValue) {
                setState(() {
                  _selectedTier = newValue;
                });
              },
              availableTiers: _tiers!,
              updateTierOfIdentity: _reloadIdentity,
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _reloadIdentity() async {
    final clientDetails = await GetIt.I.get<AdminApiClient>().clients.getClient(widget.address);
    if (mounted) {
      setState(() {
        _clientDetails = clientDetails.data;
        _selectedTier = _clientDetails!.defaultTier;
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

class _ClientDetailsCard extends StatelessWidget {
  final Client clientDetails;
  final String? selectedTier;
  final ValueChanged<String?>? onTierChanged;
  final List<TierOverview> availableTiers;
  final VoidCallback updateTierOfIdentity;

  const _ClientDetailsCard({
    required this.clientDetails,
    required this.availableTiers,
    required this.updateTierOfIdentity,
    this.selectedTier,
    this.onTierChanged,
  });

  @override
  Widget build(BuildContext context) {
    final currentTier = availableTiers.firstWhere((tier) => tier.id == clientDetails.defaultTier);

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
                      Text(clientDetails.clientId, style: Theme.of(context).textTheme.headlineLarge),
                      Gaps.w16,
                      CopyToClipboardButton(
                        clipboardText: clientDetails.clientId,
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
                      EntityDetails(
                        title: context.l10n.identityDetails_card_publicKey,
                        value: clientDetails.displayName.ellipsize(20),
                        onIconPressed: () => context.setClipboardDataWithSuccessNotification(
                          clipboardText: clientDetails.displayName,
                          successMessage: context.l10n.identityDetails_card_publicKey_copyToClipboardMessage,
                        ),
                        icon: Icons.copy,
                        tooltipMessage: context.l10n.identityDetails_card_publicKey_tooltipMessage,
                      ),
                      EntityDetails(
                        title: context.l10n.createdAt,
                        value:
                            '${DateFormat.yMd(Localizations.localeOf(context).languageCode).format(clientDetails.createdAt)} ${DateFormat.Hms().format(clientDetails.createdAt)}',
                      ),
                      EntityDetails(
                        title: context.l10n.tier,
                        value: currentTier.name,
                        onIconPressed: currentTier.canBeManuallyAssigned
                            ? () => showChangeTierDialog(
                                  context: context,
                                  onTierUpdated: updateTierOfIdentity,
                                  clientDetails: clientDetails,
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
          ),
        ),
      ],
    );
  }
}
