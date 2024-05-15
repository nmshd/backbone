import 'dart:io';

import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:admin_ui/core/core.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:intl/intl.dart';

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
    if (_identityDetails == null) return const Center(child: CircularProgressIndicator());

    final identityDetails = _identityDetails!;
    return Scrollbar(
      controller: _scrollController,
      child: SingleChildScrollView(
        controller: _scrollController,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (Platform.isMacOS || Platform.isWindows) const BackButton(),
            Row(
              children: [
                Expanded(
                  child: Card(
                    child: Padding(
                      padding: const EdgeInsets.all(16),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          const Text(
                            'Identities Overview',
                            style: TextStyle(fontSize: 32),
                          ),
                          Gaps.h32,
                          Row(
                            children: [
                              Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                children: [
                                  Text(
                                    'Address',
                                    style: Theme.of(context).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.bold),
                                  ),
                                  Text(
                                    identityDetails.address,
                                  ),
                                ],
                              ),
                              Gaps.w16,
                              Column(
                                children: [
                                  Text(
                                    'Client ID',
                                    style: Theme.of(context).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.bold),
                                  ),
                                  Text(
                                    identityDetails.clientId,
                                  ),
                                ],
                              ),
                              Gaps.w16,
                              Column(
                                children: [
                                  Text(
                                    'Public Key',
                                    style: Theme.of(context).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.bold),
                                  ),
                                  Text(
                                    identityDetails.publicKey,
                                  ),
                                ],
                              ),
                              Gaps.w16,
                              Column(
                                children: [
                                  Text(
                                    'Created at',
                                    style: Theme.of(context).textTheme.bodyMedium!.copyWith(fontWeight: FontWeight.bold),
                                  ),
                                  Text(
                                    DateFormat('yyyy-MM-dd hh:MM:ss').format(identityDetails.createdAt),
                                  ),
                                ],
                              ),
                            ],
                          ),
                          Gaps.h16,
                          Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                'Tier',
                                style: Theme.of(context).textTheme.bodyMedium!.copyWith(fontWeight: FontWeight.bold),
                              ),
                              Gaps.h16,
                              Wrap(
                                spacing: 8,
                                runSpacing: 4,
                                children: _tiers != null && _tiers!.isNotEmpty
                                    ? _tiers!.where((tier) => tier.canBeManuallyAssigned || tier.id == _identityDetails!.tierId).map((tier) {
                                        final isDisabled = _isTierDisabled(tier);
                                        return isDisabled
                                            ? ChoiceChip(
                                                label: Text(tier.name),
                                                selected: false,
                                                disabledColor: Theme.of(context).disabledColor,
                                              )
                                            : ChoiceChip(
                                                label: Text(tier.name),
                                                selected: _selectedTier == tier.id,
                                                onSelected: (bool selected) {
                                                  setState(() {
                                                    _selectedTier = selected ? tier.id : _selectedTier;
                                                  });
                                                },
                                              );
                                      }).toList()
                                    : [const Text('No tiers found.')],
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
            Padding(
              padding: const EdgeInsets.all(8),
              child: Row(
                crossAxisAlignment: CrossAxisAlignment.end,
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  ElevatedButton(
                    style: ButtonStyle(
                      shape: MaterialStateProperty.all<RoundedRectangleBorder>(
                        RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(0),
                        ),
                      ),
                      backgroundColor:
                          _selectedTier != _identityDetails!.tierId ? MaterialStateProperty.all<Color>(Theme.of(context).colorScheme.primary) : null,
                    ),
                    onPressed: _selectedTier != _identityDetails!.tierId ? _updateIdentity : null,
                    child: Text(
                      context.l10n.save,
                      style: _selectedTier != _identityDetails!.tierId ? TextStyle(color: Theme.of(context).colorScheme.onPrimary) : null,
                    ),
                  ),
                  const SizedBox(width: 16),
                  ElevatedButton(
                    style: ButtonStyle(
                      shape: MaterialStateProperty.all<RoundedRectangleBorder>(
                        RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(0),
                        ),
                      ),
                      backgroundColor: MaterialStateProperty.all<Color>(Theme.of(context).colorScheme.secondary),
                    ),
                    onPressed: () {
                      Navigator.of(context).pop();
                    },
                    child: Text(
                      context.l10n.cancel,
                      style: TextStyle(color: Theme.of(context).colorScheme.onSecondary),
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  bool _isTierDisabled(TierOverview tier) {
    if (_tiers == null || _identityDetails == null) {
      return false;
    }
    final tiersThatCannotBeUnassigned = _tiers!.where((t) => !t.canBeManuallyAssigned);
    final identityIsInTierThatCannotBeUnassigned = tiersThatCannotBeUnassigned.any((t) => t.id == _identityDetails!.tierId);
    return identityIsInTierThatCannotBeUnassigned && tier.id != _identityDetails!.tierId;
  }

  Future<void> _updateIdentity() async {
    if (_identityDetails == null) return;

    await GetIt.I.get<AdminApiClient>().identities.updateIdentity(_identityDetails!.address, tierId: _selectedTier!);

    if (mounted) {
      setState(() {
        _selectedTier = _identityDetails!.tierId;
      });
    }

    await _reloadIdentity();
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
    if (mounted) setState(() => _tiers = tiers.data);
  }
}
