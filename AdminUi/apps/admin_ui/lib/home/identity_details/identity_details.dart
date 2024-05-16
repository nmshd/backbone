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
  static const noTiersFoundMessage = 'No tiers found.';

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
                            style: TextStyle(fontSize: 40),
                          ),
                          Gaps.h32,
                          Row(
                            children: [
                              Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text(
                                    'Address',
                                    style: Theme.of(context).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.bold, fontSize: 20),
                                  ),
                                  Text(
                                    identityDetails.address,
                                    style: const TextStyle(fontSize: 18),
                                  ),
                                ],
                              ),
                              Gaps.w16,
                              Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text(
                                    'Client ID',
                                    style: Theme.of(context).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.bold, fontSize: 20),
                                  ),
                                  Text(
                                    identityDetails.clientId,
                                    style: const TextStyle(fontSize: 18),
                                  ),
                                ],
                              ),
                              Gaps.w16,
                              Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text(
                                    'Public Key',
                                    style: Theme.of(context).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.bold, fontSize: 20),
                                  ),
                                  Text(
                                    identityDetails.publicKey,
                                    style: const TextStyle(fontSize: 18),
                                  ),
                                ],
                              ),
                              Gaps.w16,
                              Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text(
                                    'Created at',
                                    style: Theme.of(context).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.bold, fontSize: 20),
                                  ),
                                  Text(
                                    DateFormat('yyyy-MM-dd hh:MM:ss').format(identityDetails.createdAt),
                                    style: const TextStyle(fontSize: 18),
                                  ),
                                ],
                              ),
                              Gaps.w16,
                              Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text(
                                    'Tier',
                                    style: Theme.of(context).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.bold, fontSize: 20),
                                  ),
                                  DropdownButton<String>(
                                    isDense: true,
                                    value: _selectedTier,
                                    onChanged: (String? newValue) {
                                      setState(() {
                                        _selectedTier = newValue;
                                      });
                                    },
                                    items: _tiers!.isNotEmpty
                                        ? _tiers!.where(_isTierManuallyAssignable).map((tier) {
                                            final isDisabled = _isTierDisabled(tier);
                                            return DropdownMenuItem<String>(
                                              value: tier.id,
                                              enabled: !isDisabled,
                                              child: isDisabled
                                                  ? Text(
                                                      tier.name,
                                                      style: TextStyle(
                                                        color: Theme.of(context).disabledColor,
                                                        fontSize: 18,
                                                      ),
                                                    )
                                                  : Text(
                                                      tier.name,
                                                      style: const TextStyle(
                                                        fontSize: 18,
                                                      ),
                                                    ),
                                            );
                                          }).toList()
                                        : [
                                            const DropdownMenuItem<String>(
                                              value: noTiersFoundMessage,
                                              child: Text(
                                                noTiersFoundMessage,
                                                style: TextStyle(
                                                  fontSize: 14,
                                                ),
                                              ),
                                            ),
                                          ],
                                  ),
                                ],
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
                      shape: WidgetStateProperty.all<RoundedRectangleBorder>(
                        RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(0),
                        ),
                      ),
                      backgroundColor:
                          _selectedTier != _identityDetails!.tierId ? WidgetStateProperty.all<Color>(Theme.of(context).colorScheme.primary) : null,
                    ),
                    onPressed: _selectedTier != _identityDetails!.tierId ? _updateIdentity : null,
                    child: Text(
                      context.l10n.save,
                      style: _selectedTier != _identityDetails!.tierId ? TextStyle(color: Theme.of(context).colorScheme.onPrimary) : null,
                    ),
                  ),
                  Gaps.w16,
                  ElevatedButton(
                    style: ButtonStyle(
                      shape: WidgetStateProperty.all<RoundedRectangleBorder>(
                        RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(0),
                        ),
                      ),
                      backgroundColor: WidgetStateProperty.all<Color>(Theme.of(context).colorScheme.secondary),
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

  bool _isTierManuallyAssignable(TierOverview tier) {
    return tier.canBeManuallyAssigned || tier.id == _identityDetails!.tierId;
  }

  Future<void> _updateIdentity() async {
    if (_identityDetails == null) return;

    final scaffoldMessenger = ScaffoldMessenger.of(context);

    try {
      await GetIt.I.get<AdminApiClient>().identities.updateIdentity(_identityDetails!.address, tierId: _selectedTier!);

      scaffoldMessenger.showSnackBar(
        const SnackBar(
          content: Text('Identity updated successfully. Reloading..'),
          duration: Duration(seconds: 3),
        ),
      );

      await _reloadIdentity();
    } catch (e) {
      scaffoldMessenger.showSnackBar(
        const SnackBar(
          content: Text('Failed to update identity. Please try again.'),
          duration: Duration(seconds: 3),
        ),
      );
    }
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
