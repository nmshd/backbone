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

    return Scrollbar(
      controller: _scrollController,
      child: SingleChildScrollView(
        controller: _scrollController,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (Platform.isMacOS || Platform.isWindows) const BackButton(),
            _buildIdentityCard(context),
          ],
        ),
      ),
    );
  }

  Widget _buildIdentityCard(BuildContext context) {
    final identityDetails = _identityDetails!;

    return Row(
      children: [
        Expanded(
          child: Card(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  _IdentityDetails(
                    title: 'Address',
                    value: identityDetails.address,
                  ),
                  Gaps.h32,
                  Wrap(
                    spacing: 8,
                    runSpacing: 8,
                    children: [
                      _IdentityDetails(
                        title: 'Client ID',
                        value: identityDetails.clientId,
                      ),
                      _IdentityDetails(
                        title: 'Public Key',
                        value: identityDetails.publicKey,
                      ),
                      _IdentityDetails(
                        title: 'Created at',
                        value: DateFormat('yyyy-MM-dd hh:MM:ss').format(identityDetails.createdAt),
                      ),
                      _buildTierDropdown(context, identityDetails),
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

  Widget _buildTierDropdown(BuildContext context, Identity identityDetails) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Tier: ',
          style: Theme.of(context).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.bold),
        ),
        DropdownButton<String>(
          isDense: true,
          value: _selectedTier,
          onChanged: (String? newValue) {
            setState(() {
              _selectedTier = newValue;
            });
            if (_selectedTier != identityDetails.tierId) {
              _updateIdentity();
            }
          },
          items: _buildTierDropdownItems(context),
        ),
      ],
    );
  }

  List<DropdownMenuItem<String>> _buildTierDropdownItems(BuildContext context) {
    if (_tiers == null || _tiers!.isEmpty) {
      return [
        const DropdownMenuItem<String>(
          value: noTiersFoundMessage,
          child: Text(
            noTiersFoundMessage,
            style: TextStyle(fontSize: 10),
          ),
        ),
      ];
    }

    return _tiers!.where(_isTierManuallyAssignable).map((tier) {
      final isDisabled = _isTierDisabled(tier);
      return DropdownMenuItem<String>(
        value: tier.id,
        enabled: !isDisabled,
        child: Text(
          tier.name,
          style: TextStyle(
            color: isDisabled ? Theme.of(context).disabledColor : null,
            fontSize: 18,
          ),
        ),
      );
    }).toList();
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
    if (mounted) {
      setState(() => _tiers = tiers.data);
    }
  }
}

class _IdentityDetails extends StatelessWidget {
  final String title;
  final String value;

  const _IdentityDetails({required this.title, required this.value});

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text.rich(
          TextSpan(
            children: [
              TextSpan(text: '$title: ', style: Theme.of(context).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.bold)),
              TextSpan(text: value, style: Theme.of(context).textTheme.bodyLarge),
            ],
          ),
        ),
      ],
    );
  }
}
