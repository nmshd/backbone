import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:intl/intl.dart';
import 'package:multi_dropdown/multiselect_dropdown.dart';

import '/core/core.dart';

class IdentitiesFilter extends StatefulWidget {
  const IdentitiesFilter({
    required this.onFilterChanged,
    super.key,
  });

  final Future<void> Function({IdentityOverviewFilter? filter}) onFilterChanged;

  @override
  State<IdentitiesFilter> createState() => _IdentitiesFilterState();
}

class _IdentitiesFilterState extends State<IdentitiesFilter> {
  IdentityOverviewFilter filter = IdentityOverviewFilter();

  late MultiSelectController<String> _tierController;
  late MultiSelectController<String> _clientController;

  final operators = <String>['=', '<', '>', '<=', '>='];
  final Map<String, FilterOperator> operatorMap = {
    '=': FilterOperator.equal,
    '<': FilterOperator.lessThan,
    '>': FilterOperator.greaterThan,
    '<=': FilterOperator.lessThanOrEqual,
    '>=': FilterOperator.greaterThanOrEqual,
  };

  @override
  void initState() {
    super.initState();
    _tierController = MultiSelectController();
    _clientController = MultiSelectController();
    loadTiers().then((_) {
      setState(() {});
    });
    loadClients().then((_) {
      setState(() {});
    });
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
                filter = filter.copyWith(address: enteredText.isEmpty ? const Optional(null) : Optional(enteredText));
                widget.onFilterChanged(filter: filter);
              },
            ),
            Gaps.w16,
            MultiSelectFilter(
              label: 'Tiers',
              searchLabel: 'Search Tiers',
              controller: _tierController,
              onOptionSelected: (List<ValueItem<String>> selectedOptions) {
                final selectedTiers = selectedOptions.map((item) => item.value!).toList();
                filter = filter.copyWith(tiers: selectedTiers.isEmpty ? const Optional.absent() : Optional(selectedTiers));
                widget.onFilterChanged(filter: filter);
              },
            ),
            Gaps.w16,
            MultiSelectFilter(
              label: 'Clients',
              searchLabel: 'Search Clients',
              controller: _clientController,
              onOptionSelected: (List<ValueItem<String>> selectedOptions) {
                final selectedClients = selectedOptions.map((item) => item.value!).toList();
                filter = filter.copyWith(clients: selectedClients.isEmpty ? const Optional.absent() : Optional(selectedClients));
                widget.onFilterChanged(filter: filter);
              },
            ),
            Gaps.w16,
            NumberFilter(
              operators: operators,
              label: 'Number of Devices',
              onNumberSelected: (String operator, String enteredValue) {
                final numberOfDevices = FilterOperatorValue(findCorrectOperator(operator)!, enteredValue);
                filter = filter.copyWith(numberOfDevices: numberOfDevices.value.isEmpty ? const Optional.absent() : Optional(numberOfDevices));
                widget.onFilterChanged(filter: filter);
              },
            ),
            Gaps.w16,
            DateFilter(
              operators: operators,
              label: 'Created At',
              onDateSelected: (DateTime? selectedDate, String operator) {
                final createdAt = FilterOperatorValue(
                  findCorrectOperator(operator)!,
                  selectedDate != null ? DateFormat('yyyy-MM-dd').format(selectedDate) : '',
                );
                filter = filter.copyWith(createdAt: createdAt.value.isEmpty ? const Optional.absent() : Optional(createdAt));
                widget.onFilterChanged(filter: filter);
              },
            ),
            Gaps.w16,
            DateFilter(
              operators: operators,
              label: 'Last Login At',
              onDateSelected: (DateTime? selectedDate, String operator) {
                final lastLoginAt = FilterOperatorValue(
                  findCorrectOperator(operator)!,
                  selectedDate != null ? DateFormat('yyyy-MM-dd').format(selectedDate) : '',
                );
                filter = filter.copyWith(lastLoginAt: lastLoginAt.value.isEmpty ? const Optional.absent() : Optional(lastLoginAt));
                widget.onFilterChanged(filter: filter);
              },
            ),
            Gaps.w16,
            NumberFilter(
              operators: operators,
              label: 'Datawallet Version',
              onNumberSelected: (String operator, String enteredValue) {
                final datawalletVersion = FilterOperatorValue(findCorrectOperator(operator)!, enteredValue);
                filter = filter.copyWith(
                  datawalletVersion: datawalletVersion.value.isEmpty ? const Optional.absent() : Optional(datawalletVersion),
                );
                widget.onFilterChanged(filter: filter);
              },
            ),
            Gaps.w16,
            NumberFilter(
              operators: operators,
              label: 'Identity Version',
              onNumberSelected: (String operator, String enteredValue) {
                final identityVersion = FilterOperatorValue(findCorrectOperator(operator)!, enteredValue);
                filter = filter.copyWith(
                  identityVersion: identityVersion.value.isEmpty ? const Optional.absent() : Optional(identityVersion),
                );
                widget.onFilterChanged(filter: filter);
              },
            ),
          ],
        ),
      ),
    );
  }

  FilterOperator? findCorrectOperator(String operator) {
    return operatorMap[operator];
  }

  Future<void> loadTiers() async {
    final response = await GetIt.I.get<AdminApiClient>().tiers.getTiers();
    final tierItems = response.data.map((tier) => ValueItem(label: tier.name, value: tier.id)).toList();
    setState(() => _tierController.setOptions(tierItems));
  }

  Future<void> loadClients() async {
    final response = await GetIt.I.get<AdminApiClient>().clients.getClients();
    final clientItems = response.data.map((client) => ValueItem(label: client.displayName, value: client.clientId)).toList();
    setState(() => _clientController.setOptions(clientItems));
  }
}
