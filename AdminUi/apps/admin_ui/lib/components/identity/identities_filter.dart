import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_ui/components/shared/shared.dart';
import 'package:admin_ui/core/constants.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:multi_dropdown/multiselect_dropdown.dart';

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

  late MultiSelectController<dynamic> _tierController;
  late MultiSelectController<dynamic> _clientController;

  late String _enteredIdentityAddress;
  late List<String> _selectedTiers;
  late List<String> _selectedClients;
  late DateTime _selectedCreatedAt;
  late String _selectedCreatedAtOperator;
  late DateTime _selectedLastLoginAt;
  late String _selectedLastLoginAtOperator;
  late String _numberOfDevicesOperator;
  late String _numberOfDevices;
  late String _datawalletVersionOperator;
  late String _dataWalletVersion;
  late String _identityVersionOperator;
  late String _identityVersion;

  late bool isCreatedAtSelected;
  late bool isLastLoginAtSelected;

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
    _selectedCreatedAt = DateTime.now();
    _selectedLastLoginAt = DateTime.now();
    _selectedCreatedAtOperator = '=';
    _selectedLastLoginAtOperator = '=';
    _numberOfDevicesOperator = '=';
    _numberOfDevices = '';
    _datawalletVersionOperator = '=';
    _dataWalletVersion = '';
    _identityVersionOperator = '=';
    _identityVersion = '';
    isCreatedAtSelected = false;
    isLastLoginAtSelected = false;
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
    return SingleChildScrollView(
      scrollDirection: Axis.horizontal,
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 5),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            InputField(
              title: 'Address',
              onEnteredText: (String enteredText) {
                _enteredIdentityAddress = enteredText;

                sendFilters();
              },
            ),
            Gaps.w16,
            SizedBox(
              width: 250,
              child: MultiSelectDropDown(
                hint: 'Tiers',
                searchLabel: 'Search Tiers',
                searchEnabled: true,
                controller: _tierController,
                options: _tierController.options,
                fieldBackgroundColor: Theme.of(context).colorScheme.background,
                searchBackgroundColor: Theme.of(context).colorScheme.background,
                dropdownBackgroundColor: Theme.of(context).colorScheme.background,
                selectedOptionBackgroundColor: Theme.of(context).colorScheme.background,
                selectedOptionTextColor: Theme.of(context).colorScheme.onBackground,
                optionsBackgroundColor: Theme.of(context).colorScheme.background,
                optionTextStyle: TextStyle(color: Theme.of(context).colorScheme.onBackground),
                onOptionSelected: (List<ValueItem<dynamic>> selectedOptions) {
                  _selectedTiers = selectedOptions.map((item) => item.value as String).toList();
                  sendFilters();
                },
              ),
            ),
            Gaps.w16,
            SizedBox(
              width: 200,
              child: MultiSelectDropDown(
                hint: 'Clients',
                searchLabel: 'Search Clients',
                searchEnabled: true,
                controller: _clientController,
                options: _clientController.options,
                fieldBackgroundColor: Theme.of(context).colorScheme.background,
                searchBackgroundColor: Theme.of(context).colorScheme.background,
                dropdownBackgroundColor: Theme.of(context).colorScheme.background,
                selectedOptionBackgroundColor: Theme.of(context).colorScheme.background,
                selectedOptionTextColor: Theme.of(context).colorScheme.onBackground,
                optionsBackgroundColor: Theme.of(context).colorScheme.background,
                optionTextStyle: TextStyle(color: Theme.of(context).colorScheme.onBackground),
                onOptionSelected: (List<ValueItem<dynamic>> selectedOptions) {
                  _selectedClients = selectedOptions.map((item) => item.value as String).toList();
                  sendFilters();
                },
              ),
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
              onDateSelected: (DateTime selectedDate, String operator, {bool isDateSelected = false}) {
                setState(() {
                  _selectedCreatedAt = selectedDate;
                  _selectedCreatedAtOperator = operator;
                  isCreatedAtSelected = isDateSelected;

                  sendFilters();
                });
              },
            ),
            Gaps.w16,
            DateFilter(
              operators: operators,
              label: 'Last Login At',
              onDateSelected: (DateTime selectedDate, String operator, {bool isDateSelected = false}) {
                setState(() {
                  _selectedLastLoginAt = selectedDate;
                  _selectedLastLoginAtOperator = operator;
                  isLastLoginAtSelected = isDateSelected;

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

    if (_selectedCreatedAtOperator.isNotEmpty && _selectedCreatedAt.toString().isNotEmpty && isCreatedAtSelected) {
      final createdAtValue = FilterOperatorValue(findCorrectOperator(_selectedCreatedAtOperator)!, _selectedCreatedAt.toString().substring(0, 10));
      filter = filter.copyWith(createdAt: createdAtValue);
    }
    if (_selectedLastLoginAtOperator.isNotEmpty && _selectedLastLoginAt.toString().isNotEmpty && isLastLoginAtSelected) {
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
