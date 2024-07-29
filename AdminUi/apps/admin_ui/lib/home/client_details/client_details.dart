import 'dart:io';

import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:intl/intl.dart';

import '/core/core.dart';

class ClientDetails extends StatefulWidget {
  final String clientId;

  const ClientDetails({required this.clientId, super.key});

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

    _reloadClient();
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
              updateTierOfIdentity: _reloadClient,
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _reloadClient() async {
    final clientDetails = await GetIt.I.get<AdminApiClient>().clients.getClient(widget.clientId);
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

    return Column(
      children: [
        Row(
          children: [
            Expanded(
              child: Card(
                child: Padding(
                  padding: const EdgeInsets.all(16),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Gaps.h32,
                      Wrap(
                        crossAxisAlignment: WrapCrossAlignment.center,
                        spacing: 8,
                        runSpacing: 8,
                        children: [
                          EntityDetails(title: context.l10n.id, value: clientDetails.clientId),
                          EntityDetails(
                            title: context.l10n.maxIdentities,
                            value: '${clientDetails.maxIdentities == 0 ? 0 : clientDetails.maxIdentities}',
                          ),
                          EntityDetails(
                            title: context.l10n.createdAt,
                            value:
                                '${DateFormat.yMd(Localizations.localeOf(context).languageCode).format(clientDetails.createdAt)} ${DateFormat.Hms().format(clientDetails.createdAt)}',
                          ),
                          EntityDetails(
                            title: context.l10n.clientDetails_card_defaultTier,
                            value: currentTier.name,
                            onIconPressed: currentTier.canBeManuallyAssigned || currentTier.canBeUsedAsDefaultForClient
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
        ),
        Gaps.h16,
        IdentitiesList(clientDetails: clientDetails),
      ],
    );
  }
}
