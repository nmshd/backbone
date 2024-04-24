import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
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
  late ScrollController _scrollController;
  IdentityOverviewFilter filter = IdentityOverviewFilter();

  late MultiSelectController<String> _tierController;
  late MultiSelectController<String> _clientController;

  late String _enteredIdentityAddress;
  late List<String> _selectedTiers;
  late List<String> _selectedClients;
  DateTime? _selectedCreatedAt;
  late String _selectedCreatedAtOperator;
  DateTime? _selectedLastLoginAt;
  late String _selectedLastLoginAtOperator;
  late String _numberOfDevicesOperator;
  late String _numberOfDevices;
  late String _datawalletVersionOperator;
  late String _dataWalletVersion;
  late String _identityVersionOperator;
  late String _identityVersion;

  final operators = <String>['=', '<', '>', '<=', '>='];
  final List<_Operator> comparableOperators = [
    _Operator(operator: FilterOperator.equal, value: '='),
    _Operator(operator: FilterOperator.lessThan, value: '<'),
    _Operator(operator: FilterOperator.greaterThan, value: '>'),
    _Operator(operator: FilterOperator.lessThanOrEqual, value: '<='),
    _Operator(operator: FilterOperator.greaterThanOrEqual, value: '>='),
  ];

  @override
  void initState() {
    super.initState();
    _tierController = MultiSelectController();
    _clientController = MultiSelectController();
    _scrollController = ScrollController();
    _enteredIdentityAddress = '';
    _selectedTiers = [];
    _selectedClients = [];
    _selectedCreatedAtOperator = '=';
    _selectedLastLoginAtOperator = '=';
    _numberOfDevicesOperator = '=';
    _numberOfDevices = '';
    _datawalletVersionOperator = '=';
    _dataWalletVersion = '';
    _identityVersionOperator = '=';
    _identityVersion = '';
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
    _scrollController.dispose();
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
                _enteredIdentityAddress = enteredText;

                sendFilters();
              },
            ),
            Gaps.w16,
            MultiSelectFilter(
              label: 'Tiers',
              searchLabel: 'Search Tiers',
              controller: _tierController,
              onOptionSelected: (List<ValueItem<String>> selectedOptions) {
                _selectedTiers = selectedOptions.map((item) => item.value!).toList();
                sendFilters();
              },
            ),
            Gaps.w16,
            MultiSelectFilter(
              label: 'Clients',
              searchLabel: 'Search Clients',
              controller: _clientController,
              onOptionSelected: (List<ValueItem<String>> selectedOptions) {
                _selectedClients = selectedOptions.map((item) => item.value!).toList();
                sendFilters();
              },
            ),
            Gaps.w16,
            NumberFilter(
              operators: operators,
              label: 'Number of Devices',
              onNumberSelected: (String operator, String enteredValue) {
                _numberOfDevices = enteredValue;
                _numberOfDevicesOperator = operator;

                sendFilters();
              },
            ),
            Gaps.w16,
            DateFilter(
              operators: operators,
              label: 'Created At',
              onDateSelected: (DateTime? selectedDate, String operator) {
                setState(() {
                  if (selectedDate != null) {
                    _selectedCreatedAt = selectedDate;
                  }
                  _selectedCreatedAtOperator = operator;

                  sendFilters();
                });
              },
            ),
            Gaps.w16,
            DateFilter(
              operators: operators,
              label: 'Last Login At',
              onDateSelected: (DateTime? selectedDate, String operator) {
                setState(() {
                  if (selectedDate != null) {
                    _selectedLastLoginAt = selectedDate;
                  }
                  _selectedLastLoginAtOperator = operator;
                  sendFilters();
                });
              },
            ),
            Gaps.w16,
            NumberFilter(
              operators: operators,
              label: 'Datawallet Version',
              onNumberSelected: (String operator, String enteredValue) {
                _dataWalletVersion = enteredValue;
                _datawalletVersionOperator = operator;

                sendFilters();
              },
            ),
            Gaps.w16,
            NumberFilter(
              operators: operators,
              label: 'Identity Version',
              onNumberSelected: (String operator, String enteredValue) {
                _identityVersion = enteredValue;
                _identityVersionOperator = operator;

                sendFilters();
              },
            ),
          ],
        ),
      ),
    );
  }

  FilterOperator? findCorrectOperator(String operator) {
    for (final o in comparableOperators) {
      if (o.value == operator) {
        return o.operator;
      }
    }
    return null;
  }

  void sendFilters() {
    filter = IdentityOverviewFilter();

    if (_enteredIdentityAddress.isNotEmpty) {
      filter = filter.copyWith(address: _enteredIdentityAddress);
    }
    if (_selectedTiers.isNotEmpty) {
      filter = filter.copyWith(tiers: _selectedTiers);
    }
    if (_selectedClients.isNotEmpty) {
      filter = filter.copyWith(clients: _selectedClients);
    }

    if (_selectedCreatedAt != null && _selectedCreatedAtOperator.isNotEmpty) {
      final createdAtValue = FilterOperatorValue(findCorrectOperator(_selectedCreatedAtOperator)!, _selectedCreatedAt.toString().substring(0, 10));
      filter = filter.copyWith(createdAt: createdAtValue);
    }
    if (_selectedLastLoginAt != null && _selectedLastLoginAtOperator.isNotEmpty) {
      final lastLoginAtValue =
          FilterOperatorValue(findCorrectOperator(_selectedLastLoginAtOperator)!, _selectedLastLoginAt.toString().substring(0, 10));
      filter = filter.copyWith(lastLoginAt: lastLoginAtValue);
    }

    if (_numberOfDevices.isNotEmpty) {
      final numberOfDevicesValue = FilterOperatorValue(findCorrectOperator(_numberOfDevicesOperator)!, _numberOfDevices);
      filter = filter.copyWith(numberOfDevices: numberOfDevicesValue);
    }
    if (_dataWalletVersion.isNotEmpty) {
      final datawalletVersionValue = FilterOperatorValue(findCorrectOperator(_datawalletVersionOperator)!, _dataWalletVersion);
      filter = filter.copyWith(datawalletVersion: datawalletVersionValue);
    }
    if (_identityVersion.isNotEmpty) {
      final identityVersionValue = FilterOperatorValue(findCorrectOperator(_identityVersionOperator)!, _identityVersion);
      filter = filter.copyWith(identityVersion: identityVersionValue);
    }

    setState(() {
      widget.onFilterChanged(filter: filter);
    });
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

class _Operator {
  FilterOperator operator;
  String value;

  _Operator({required this.operator, required this.value});
}
