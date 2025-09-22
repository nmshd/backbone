import 'dart:async';

import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:enmeshed_ui_kit/enmeshed_ui_kit.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';
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

    unawaited(_reloadClient());
    unawaited(_reloadTiers());
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
            if (kIsDesktop)
              Row(
                children: [
                  const BackButton(),
                  IconButton(
                    icon: const Icon(Icons.refresh),
                    onPressed: () async {
                      await _reloadClient();
                      await _reloadTiers();
                    },
                    tooltip: context.l10n.reload,
                  ),
                ],
              ),
            _ClientDetailsCard(clientDetails: clientDetails, selectedTier: _selectedTier, availableTiers: _tiers!, updateClient: _reloadClient),
            Gaps.h16,
            IdentitiesTable(clientDetails: clientDetails),
          ],
        ),
      ),
    );
  }

  Future<void> _reloadClient() async {
    final clientDetails = await GetIt.I.get<AdminApiClient>().clients.getClient(widget.clientId);

    if (!mounted) return;

    if (clientDetails.hasError) return context.pushReplacement('/error', extra: clientDetails.error.message);

    setState(() {
      _clientDetails = clientDetails.data;
      _selectedTier = _clientDetails!.defaultTier;
    });
  }

  Future<void> _reloadTiers() async {
    final tiers = await GetIt.I.get<AdminApiClient>().tiers.getTiers();
    if (mounted) setState(() => _tiers = tiers.data);
  }
}

class _ClientDetailsCard extends StatelessWidget {
  final Client clientDetails;
  final String? selectedTier;
  final List<TierOverview> availableTiers;
  final VoidCallback updateClient;

  const _ClientDetailsCard({required this.clientDetails, required this.availableTiers, required this.updateClient, this.selectedTier});

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
                  child: Wrap(
                    crossAxisAlignment: WrapCrossAlignment.center,
                    spacing: 8,
                    runSpacing: 8,
                    children: [
                      CopyableEntityDetails(title: context.l10n.id, value: clientDetails.clientId),
                      EntityDetails(title: context.l10n.displayName, value: clientDetails.displayName),
                      EntityDetails(
                        title: context.l10n.maxIdentities,
                        value: '${clientDetails.maxIdentities ?? context.l10n.noLimit}',
                        onIconPressed: () =>
                            showChangeMaxIdentitiesDialog(context: context, clientDetails: clientDetails, onMaxIdentitiesUpdated: updateClient),
                        icon: Icons.edit,
                        tooltipMessage: context.l10n.clientDetails_maxIdentities_tooltip,
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
                                onTierUpdated: updateClient,
                                clientDetails: clientDetails,
                                availableTiers: availableTiers,
                              )
                            : null,
                        icon: Icons.edit,
                        tooltipMessage: context.l10n.changeTier,
                      ),
                    ],
                  ),
                ),
              ),
            ),
          ],
        ),
      ],
    );
  }
}
