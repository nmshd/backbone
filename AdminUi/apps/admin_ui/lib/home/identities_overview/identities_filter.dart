import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:intl/intl.dart';
import 'package:multi_dropdown/multiselect_dropdown.dart';

import '/core/core.dart';

class IdentitiesFilter extends StatefulWidget {
  final Future<void> Function({IdentityOverviewFilter? filter}) onFilterChanged;

  const IdentitiesFilter({
    required this.onFilterChanged,
    super.key,
  });

  @override
  State<IdentitiesFilter> createState() => _IdentitiesFilterState();
}

class _IdentitiesFilterState extends State<IdentitiesFilter> {
  IdentityOverviewFilter _filter = IdentityOverviewFilter();

  final MultiSelectController<String> _tierController = MultiSelectController();
  final MultiSelectController<String> _clientController = MultiSelectController();

  @override
  void initState() {
    super.initState();

    _loadTiers();
    _loadClients();
  }

  @override
  void dispose() {
    _tierController.dispose();
    _clientController.dispose();

    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(8),
      child: SingleChildScrollView(
        scrollDirection: Axis.horizontal,
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            InputField(
              label: 'Address',
              onEnteredText: (String enteredText) {
                _filter = _filter.copyWith(address: enteredText.isEmpty ? const Optional.absent() : Optional(enteredText));
                widget.onFilterChanged(filter: _filter);
              },
            ),
            Gaps.w16,
            MultiSelectFilter(
              label: 'Tiers',
              searchLabel: 'Search Tiers',
              controller: _tierController,
              onOptionSelected: (List<ValueItem<String>> selectedOptions) {
                final selectedTiers = selectedOptions.map((item) => item.value!).toList();
                _filter = _filter.copyWith(tiers: selectedTiers.isEmpty ? const Optional.absent() : Optional(selectedTiers));
                widget.onFilterChanged(filter: _filter);
              },
            ),
            Gaps.w16,
            MultiSelectFilter(
              label: 'Clients',
              searchLabel: 'Search Clients',
              controller: _clientController,
              onOptionSelected: (List<ValueItem<String>> selectedOptions) {
                final selectedClients = selectedOptions.map((item) => item.value!).toList();
                _filter = _filter.copyWith(clients: selectedClients.isEmpty ? const Optional.absent() : Optional(selectedClients));
                widget.onFilterChanged(filter: _filter);
              },
            ),
            Gaps.w16,
            NumberFilter(
              label: 'Number of Devices',
              onNumberSelected: (FilterOperator operator, String enteredValue) {
                final numberOfDevices = FilterOperatorValue(operator, enteredValue);
                _filter = _filter.copyWith(numberOfDevices: numberOfDevices.value.isEmpty ? const Optional.absent() : Optional(numberOfDevices));
                widget.onFilterChanged(filter: _filter);
              },
            ),
            Gaps.w16,
            DateFilter(
              label: 'Created At',
              onFilterSelected: (FilterOperator operator, DateTime? selectedDate) {
                final createdAt = FilterOperatorValue(operator, selectedDate != null ? DateFormat('yyyy-MM-dd').format(selectedDate) : '');
                _filter = _filter.copyWith(createdAt: createdAt.value.isEmpty ? const Optional.absent() : Optional(createdAt));
                widget.onFilterChanged(filter: _filter);
              },
            ),
            Gaps.w16,
            DateFilter(
              label: 'Last Login At',
              onFilterSelected: (FilterOperator operator, DateTime? selectedDate) {
                final lastLoginAt = FilterOperatorValue(operator, selectedDate != null ? DateFormat('yyyy-MM-dd').format(selectedDate) : '');
                _filter = _filter.copyWith(lastLoginAt: lastLoginAt.value.isEmpty ? const Optional.absent() : Optional(lastLoginAt));
                widget.onFilterChanged(filter: _filter);
              },
            ),
            Gaps.w16,
            NumberFilter(
              label: 'Datawallet Version',
              onNumberSelected: (FilterOperator operator, String enteredValue) {
                final datawalletVersion = FilterOperatorValue(operator, enteredValue);
                _filter =
                    _filter.copyWith(datawalletVersion: datawalletVersion.value.isEmpty ? const Optional.absent() : Optional(datawalletVersion));
                widget.onFilterChanged(filter: _filter);
              },
            ),
            Gaps.w16,
            NumberFilter(
              label: 'Identity Version',
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
    final tierItems = response.data.map((tier) => ValueItem(label: tier.name, value: tier.id)).toList();
    setState(() => _tierController.setOptions(tierItems));
  }

  Future<void> _loadClients() async {
    final response = await GetIt.I.get<AdminApiClient>().clients.getClients();
    final clientItems = response.data.map((client) => ValueItem(label: client.displayName, value: client.clientId)).toList();
    setState(() => _clientController.setOptions(clientItems));
  }
}
