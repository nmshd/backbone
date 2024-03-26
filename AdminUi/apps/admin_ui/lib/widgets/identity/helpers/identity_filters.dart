import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:admin_ui/widgets/shared/date_filter.dart';
import 'package:admin_ui/widgets/shared/input_field.dart';
import 'package:admin_ui/widgets/shared/model/operator.dart';
import 'package:admin_ui/widgets/shared/multi_select_dialog.dart';
import 'package:admin_ui/widgets/shared/number_filter.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:multi_select_flutter/util/multi_select_item.dart';

class IdentityFilter extends StatefulWidget {
  const IdentityFilter(
    this.loadIdentities, {
    super.key,
  });

  final Future<void> Function({IdentityOverviewFilter? filter}) loadIdentities;

  @override
  State<IdentityFilter> createState() => _IdentityFilterState();
}

class _IdentityFilterState extends State<IdentityFilter> {
  List<Tier> tiers = [];
  List<Clients> clients = [];

  IdentityOverviewFilter filter = IdentityOverviewFilter();

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
  final List<Operator> comparableOperators = [
    Operator(operator: FilterOperator.equal, value: '='),
    Operator(operator: FilterOperator.lessThan, value: '<'),
    Operator(operator: FilterOperator.greaterThan, value: '>'),
    Operator(operator: FilterOperator.lessThanOrEqual, value: '<='),
    Operator(operator: FilterOperator.greaterThanOrEqual, value: '>='),
  ];

  @override
  void initState() {
    super.initState();
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
  Widget build(BuildContext context) {
    return Row(
      children: [
        InputField(
          title: 'Address',
          onEnteredText: (String enteredText) {
            _enteredIdentityAddress = enteredText;

            sendFilters();
          },
        ),
        const SizedBox(
          width: 8,
        ),
        MultiSelectDialog(
          sendFilters,
          title: 'Tier',
          multiSelectItem: tiers.map((tier) => MultiSelectItem<String>(tier.id, tier.name)).toList(),
          onSelectedValues: (List<String> selectedValues) {
            _selectedTiers = selectedValues;
            sendFilters();
          },
        ),
        const SizedBox(
          width: 8,
        ),
        MultiSelectDialog(
          sendFilters,
          title: 'Client',
          multiSelectItem: clients.map((client) => MultiSelectItem<String>(client.clientId, client.displayName)).toList(),
          onSelectedValues: (List<String> selectedValues) {
            _selectedClients = selectedValues;
            sendFilters();
          },
        ),
        const SizedBox(
          width: 8,
        ),
        NumberFilter(
          operators: operators,
          onNumberSelected: (String operator, String enteredValue) {
            _numberOfDevices = enteredValue;
            _numberOfDevicesOperator = operator;

            sendFilters();
          },
        ),
        const SizedBox(
          width: 8,
        ),
        DateFilter(
          operators: operators,
          onDateSelected: (DateTime selectedDate, String operator, {bool isDateSelected = false}) {
            setState(() {
              _selectedLastLoginAt = selectedDate;
              _selectedLastLoginAtOperator = operator;
              isLastLoginAtSelected = isDateSelected;

              sendFilters();
            });
          },
        ),
        const SizedBox(
          width: 8,
        ),
        DateFilter(
          operators: operators,
          onDateSelected: (DateTime selectedDate, String operator, {bool isDateSelected = false}) {
            setState(() {
              _selectedCreatedAt = selectedDate;
              _selectedCreatedAtOperator = operator;
              isCreatedAtSelected = isDateSelected;

              sendFilters();
            });
          },
        ),
        const SizedBox(
          width: 8,
        ),
        NumberFilter(
          operators: operators,
          onNumberSelected: (String operator, String enteredValue) {
            _dataWalletVersion = enteredValue;
            _datawalletVersionOperator = operator;

            sendFilters();
          },
        ),
        const SizedBox(
          width: 8,
        ),
        NumberFilter(
          operators: operators,
          onNumberSelected: (String operator, String enteredValue) {
            _identityVersion = enteredValue;
            _identityVersionOperator = operator;

            sendFilters();
          },
        ),
      ],
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
      filter = filter.copyWith(receivedAddress: _enteredIdentityAddress);
    }

    if (_selectedTiers.isNotEmpty) {
      filter = filter.copyWith(receivedTiers: _selectedTiers);
    }

    if (_selectedClients.isNotEmpty) {
      filter = filter.copyWith(receivedClients: _selectedClients);
    }

    if (_selectedCreatedAtOperator.isNotEmpty && _selectedCreatedAt.toString().isNotEmpty && isCreatedAtSelected) {
      final createdAtValue = FilterOperatorValue(findCorrectOperator(_selectedCreatedAtOperator)!, _selectedCreatedAt.toString().substring(0, 10));
      filter = filter.copyWith(receivedCreatedAt: createdAtValue);
    }
    if (_selectedLastLoginAtOperator.isNotEmpty && _selectedLastLoginAt.toString().isNotEmpty && isLastLoginAtSelected) {
      final lastLoginAtValue =
          FilterOperatorValue(findCorrectOperator(_selectedLastLoginAtOperator)!, _selectedLastLoginAt.toString().substring(0, 10));
      filter = filter.copyWith(receivedLastLoginAt: lastLoginAtValue);
    }

    if (_numberOfDevices.isNotEmpty) {
      final numberOfDevicesValue = FilterOperatorValue(findCorrectOperator(_numberOfDevicesOperator)!, _numberOfDevices);
      filter = filter.copyWith(receivedNumberOfDevices: numberOfDevicesValue);
    }

    if (_dataWalletVersion.isNotEmpty) {
      final datawalletVersionValue = FilterOperatorValue(findCorrectOperator(_datawalletVersionOperator)!, _dataWalletVersion);
      filter = filter.copyWith(receivedDatawalletVersion: datawalletVersionValue);
    }

    if (_identityVersion.isNotEmpty) {
      final identityVersionValue = FilterOperatorValue(findCorrectOperator(_identityVersionOperator)!, _identityVersion);
      filter = filter.copyWith(receivedIdentityVersion: identityVersionValue);
    }

    setState(() {
      widget.loadIdentities(filter: filter);
    });
  }

  Future<void> loadTiers() async {
    final response = await GetIt.I.get<AdminApiClient>().tiers.getTiers();

    tiers = response.data;
  }

  Future<void> loadClients() async {
    final response = await GetIt.I.get<AdminApiClient>().clients.getClients();

    clients = response.data;
  }
}
