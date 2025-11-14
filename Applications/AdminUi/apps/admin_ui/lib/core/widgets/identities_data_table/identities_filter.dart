import 'dart:async';

import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:intl/intl.dart';

import '../../constants.dart';
import '../../extensions.dart';
import '../filters/filters.dart';

class IdentitiesFilter extends StatefulWidget {
  final void Function({IdentityOverviewFilter? filter}) onFilterChanged;
  final String? fixedTierId;
  final String? fixedClientId;

  const IdentitiesFilter({required this.onFilterChanged, this.fixedTierId, this.fixedClientId, super.key});

  @override
  State<IdentitiesFilter> createState() => _IdentitiesFilterState();
}

class _IdentitiesFilterState extends State<IdentitiesFilter> {
  IdentityOverviewFilter _filter = IdentityOverviewFilter();

  List<MultiSelectFilterOption> _availableTiers = [];
  List<MultiSelectFilterOption> _availableClients = [];

  @override
  void initState() {
    super.initState();

    if (widget.fixedTierId != null) {
      _filter = _filter.copyWith(tiers: Optional([widget.fixedTierId!]));
    }

    unawaited(_loadTiers());
    unawaited(_loadClients());
  }

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      scrollDirection: .horizontal,
      child: Padding(
        padding: const EdgeInsets.all(8),
        child: Row(
          mainAxisSize: .min,
          children: [
            InputField(
              label: context.l10n.address,
              onEnteredText: (String enteredText) {
                _filter = _filter.copyWith(address: enteredText.isEmpty ? const Optional.absent() : Optional(enteredText));
                widget.onFilterChanged(filter: _filter);
              },
            ),
            if (widget.fixedTierId == null) ...[
              Gaps.w16,
              MultiSelectFilter(
                label: context.l10n.tiers,
                options: _availableTiers,
                onOptionSelected: (List<String> selectedTiers) {
                  _filter = _filter.copyWith(tiers: selectedTiers.isEmpty ? const Optional.absent() : Optional(selectedTiers));
                  widget.onFilterChanged(filter: _filter);
                },
              ),
            ],
            if (widget.fixedClientId == null) ...[
              Gaps.w16,
              MultiSelectFilter(
                label: context.l10n.clients,
                options: _availableClients,
                onOptionSelected: (List<String> selectedClients) {
                  _filter = _filter.copyWith(clients: selectedClients.isEmpty ? const Optional.absent() : Optional(selectedClients));
                  widget.onFilterChanged(filter: _filter);
                },
              ),
            ],
            Gaps.w16,
            NumberFilter(
              label: context.l10n.numberOfDevices,
              onNumberSelected: (FilterOperator operator, String enteredValue) {
                final numberOfDevices = FilterOperatorValue(operator, enteredValue);
                _filter = _filter.copyWith(numberOfDevices: numberOfDevices.value.isEmpty ? const Optional.absent() : Optional(numberOfDevices));
                widget.onFilterChanged(filter: _filter);
              },
            ),
            Gaps.w16,
            DateFilter(
              label: context.l10n.createdAt,
              onFilterSelected: (FilterOperator operator, DateTime? selectedDate) {
                final createdAt = FilterOperatorValue(operator, selectedDate != null ? DateFormat('yyyy-MM-dd').format(selectedDate) : '');
                _filter = _filter.copyWith(createdAt: createdAt.value.isEmpty ? const Optional.absent() : Optional(createdAt));
                widget.onFilterChanged(filter: _filter);
              },
            ),
            Gaps.w16,
            DateFilter(
              label: context.l10n.lastLoginAt,
              onFilterSelected: (FilterOperator operator, DateTime? selectedDate) {
                final lastLoginAt = FilterOperatorValue(operator, selectedDate != null ? DateFormat('yyyy-MM-dd').format(selectedDate) : '');
                _filter = _filter.copyWith(lastLoginAt: lastLoginAt.value.isEmpty ? const Optional.absent() : Optional(lastLoginAt));
                widget.onFilterChanged(filter: _filter);
              },
            ),
            Gaps.w16,
            NumberFilter(
              label: context.l10n.datawalletVersion,
              onNumberSelected: (FilterOperator operator, String enteredValue) {
                final datawalletVersion = FilterOperatorValue(operator, enteredValue);
                _filter = _filter.copyWith(
                  datawalletVersion: datawalletVersion.value.isEmpty ? const Optional.absent() : Optional(datawalletVersion),
                );
                widget.onFilterChanged(filter: _filter);
              },
            ),
            Gaps.w16,
            NumberFilter(
              label: context.l10n.identityVersion,
              onNumberSelected: (FilterOperator operator, String enteredValue) {
                final identityVersion = FilterOperatorValue(operator, enteredValue);
                _filter = _filter.copyWith(identityVersion: identityVersion.value.isEmpty ? const Optional.absent() : Optional(identityVersion));
                widget.onFilterChanged(filter: _filter);
              },
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _loadTiers() async {
    final response = await GetIt.I.get<AdminApiClient>().tiers.getTiers();
    final tierItems = response.data.map((tier) => (value: tier.id, label: tier.name)).toList();

    if (mounted) setState(() => _availableTiers = tierItems);
  }

  Future<void> _loadClients() async {
    final response = await GetIt.I.get<AdminApiClient>().clients.getClients();
    final clientItems = response.data.map((client) => (value: client.clientId, label: client.displayName)).toList();

    if (mounted) setState(() => _availableClients = clientItems);
  }
}
