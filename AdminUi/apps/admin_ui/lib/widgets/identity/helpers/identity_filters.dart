import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:admin_ui/widgets/shared/date_filter.dart';
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

  final _searchAddressController = TextEditingController();
  final _searchNumberOfDevicesController = TextEditingController();
  final _searchDatawalletVersionController = TextEditingController();
  final _searchIdentityVersionController = TextEditingController();
  late String _identityAddress;
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

  bool isCreatedAtSelected = false;
  bool isLastLoginAtSelected = false;

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
    _identityAddress = '';
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
    loadTiers().then((_) {
      setState(() {});
    });
    loadClients().then((_) {
      setState(() {});
    });
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

    if (_identityAddress.isNotEmpty) {
      filter = filter.copyWith(receivedAddress: _identityAddress);
    } else {
      filter = filter.copyWith();
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

    if (_searchNumberOfDevicesController.text.isNotEmpty) {
      final numberOfDevicesValue = FilterOperatorValue(findCorrectOperator(_numberOfDevicesOperator)!, _searchNumberOfDevicesController.text);
      filter = filter.copyWith(receivedNumberOfDevices: numberOfDevicesValue);
    }

    if (_searchDatawalletVersionController.text.isNotEmpty) {
      final datawalletVersionValue = FilterOperatorValue(findCorrectOperator(_datawalletVersionOperator)!, _searchDatawalletVersionController.text);
      filter = filter.copyWith(receivedDatawalletVersion: datawalletVersionValue);
    }

    if (_searchIdentityVersionController.text.isNotEmpty) {
      final identityVersionValue = FilterOperatorValue(findCorrectOperator(_identityVersionOperator)!, _searchIdentityVersionController.text);
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

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        SizedBox(
          width: 150,
          height: 60,
          child: TextField(
            controller: _searchAddressController,
            onChanged: (value) {
              _identityAddress = value;
              sendFilters();
            },
            decoration: const InputDecoration(
              labelText: 'Search Address',
              border: OutlineInputBorder(),
            ),
          ),
        ),
        const SizedBox(width: 8),
        MultiSelectDialog(
          sendFilters,
          title: 'Tier',
          multiSelectItem: tiers.map((tier) => MultiSelectItem<String>(tier.id, tier.name)).toList(),
          selectedValues: _selectedTiers,
        ),
        const SizedBox(width: 8),
        MultiSelectDialog(
          sendFilters,
          title: 'Client',
          multiSelectItem: clients.map((client) => MultiSelectItem<String>(client.clientId, client.displayName)).toList(),
          selectedValues: _selectedClients,
        ),
        const SizedBox(width: 8),
        DateFilter(
          sendFilters,
          operator: _selectedLastLoginAtOperator,
          operators: operators,
          isDateSelected: isCreatedAtSelected,
          date: _selectedLastLoginAt,
        ),
        DateFilter(
          sendFilters,
          operator: _selectedCreatedAtOperator,
          operators: operators,
          isDateSelected: isLastLoginAtSelected,
          date: _selectedCreatedAt,
        ),
        const SizedBox(width: 8),
        NumberFilter(
          sendFilters,
          operators: operators,
          operator: _numberOfDevicesOperator,
          controller: _searchNumberOfDevicesController,
          enteredValue: _numberOfDevices,
        ),
        const SizedBox(width: 8),
        NumberFilter(
          sendFilters,
          operators: operators,
          operator: _datawalletVersionOperator,
          controller: _searchDatawalletVersionController,
          enteredValue: _dataWalletVersion,
        ),
        const SizedBox(width: 8),
        NumberFilter(
          sendFilters,
          operators: operators,
          operator: _identityVersionOperator,
          controller: _searchIdentityVersionController,
          enteredValue: _identityVersion,
        ),
      ],
    );
  }
}
