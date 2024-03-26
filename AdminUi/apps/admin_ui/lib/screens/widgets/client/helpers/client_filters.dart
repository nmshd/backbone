import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:admin_ui/screens/widgets/shared/date_filter.dart';
import 'package:admin_ui/screens/widgets/shared/input_field.dart';
import 'package:admin_ui/screens/widgets/shared/multi_select_dialog.dart';
import 'package:admin_ui/screens/widgets/shared/number_filter.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:multi_select_flutter/util/multi_select_item.dart';

class ClientFilter extends StatefulWidget {
  const ClientFilter(
    this.filterClients, {
    super.key,
  });

  final void Function(List<Clients> clients) filterClients;

  @override
  State<ClientFilter> createState() => _ClientFilterState();
}

class _ClientFilterState extends State<ClientFilter> {
  List<Tier> tiers = [];
  List<Clients> clients = [];

  late String _enteredClientId;
  late String _enteredDisplayName;
  late List<String> _selectedTiers;
  late DateTime _selectedCreatedAt;
  late String _selectedCreatedAtOperator;
  late String _numberOfIdentitiesOperator;
  late String _numberOfIdentities;

  late bool isCreatedAtSelected;
  late bool isLastLoginAtSelected;

  final operators = <String>['=', '<', '>', '<=', '>='];

  @override
  void initState() {
    super.initState();
    _enteredClientId = '';
    _enteredDisplayName = '';
    _selectedTiers = [];
    _selectedCreatedAt = DateTime.now();
    _selectedCreatedAtOperator = '=';
    _numberOfIdentitiesOperator = '=';
    _numberOfIdentities = '';
    isCreatedAtSelected = false;
    loadTiers().then((_) {
      setState(() {});
    });
  }

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        InputField(
          title: 'Client ID',
          onEnteredText: (String enteredText) {
            setState(() {
              _enteredClientId = enteredText;
              setFilter();
            });
          },
        ),
        const SizedBox(
          width: 8,
        ),
        InputField(
          title: 'Display Name',
          onEnteredText: (String enteredText) {
            setState(() {
              _enteredDisplayName = enteredText;
              setFilter();
            });
          },
        ),
        const SizedBox(
          width: 8,
        ),
        MultiSelectDialog(
          loadTiers,
          title: 'Tier',
          multiSelectItem: tiers.map((tier) => MultiSelectItem<String>(tier.id, tier.name)).toList(),
          onSelectedValues: (List<String> selectedValues) {
            setState(() {
              _selectedTiers = selectedValues;
              setFilter();
            });
          },
        ),
        const SizedBox(
          width: 8,
        ),
        NumberFilter(
          operators: operators,
          onNumberSelected: (String operator, String enteredValue) {
            setState(() {
              _numberOfIdentities = enteredValue;
              _numberOfIdentitiesOperator = operator;

              setFilter();
            });
          },
        ),
        const SizedBox(
          height: 8,
        ),
        DateFilter(
          operators: operators,
          onDateSelected: (DateTime selectedDate, String operator, {bool isDateSelected = false}) {
            setState(() {
              setState(() {
                _selectedCreatedAt = selectedDate;
                _selectedCreatedAtOperator = operator;
                isCreatedAtSelected = isDateSelected;

                setFilter();
              });
            });
          },
        ),
      ],
    );
  }

  void setFilter() {
    var filteredClients = List<Clients>.from(clients);

    if (_enteredClientId.isNotEmpty) {
      filteredClients = filteredClients.where((client) => client.clientId == _enteredClientId).toList();
    }

    if (_enteredDisplayName.isNotEmpty) {
      filteredClients = filteredClients.where((client) => client.displayName == _enteredDisplayName).toList();
    }

    if (_selectedTiers.isNotEmpty) {
      filteredClients = filteredClients.where((client) => _selectedTiers.contains(client.defaultTier.id)).toList();
    }

    if (_numberOfIdentities.isNotEmpty) {
      switch (_numberOfIdentitiesOperator) {
        case '=':
          filteredClients = filteredClients.where((client) => client.numberOfIdentities == int.parse(_numberOfIdentities)).toList();
        case '<':
          filteredClients = filteredClients.where((client) => client.numberOfIdentities! < int.parse(_numberOfIdentities)).toList();
        case '>':
          filteredClients = filteredClients.where((client) => client.numberOfIdentities! > int.parse(_numberOfIdentities)).toList();
        case '<=':
          filteredClients = filteredClients.where((client) => client.numberOfIdentities! <= int.parse(_numberOfIdentities)).toList();
        case '>=':
          filteredClients = filteredClients.where((client) => client.numberOfIdentities! >= int.parse(_numberOfIdentities)).toList();
      }
    }

    if (_selectedCreatedAt.toString().isNotEmpty && isCreatedAtSelected) {
      switch (_selectedCreatedAtOperator) {
        case '=':
          filteredClients = filteredClients.where((client) => client.createdAt == _selectedCreatedAt.toString()).toList();
        case '<':
          filteredClients = filteredClients.where((client) => client.createdAt.compareTo(_selectedCreatedAt.toString()) < 0).toList();
        case '>':
          filteredClients = filteredClients.where((client) => client.createdAt.compareTo(_selectedCreatedAt.toString()) > 0).toList();
        case '<=':
          filteredClients = filteredClients.where((client) => client.createdAt.compareTo(_selectedCreatedAt.toString()) <= 0).toList();
        case '>=':
          filteredClients = filteredClients.where((client) => client.createdAt.compareTo(_selectedCreatedAt.toString()) >= 0).toList();
      }
    }

    setState(() {
      widget.filterClients(clients);
    });
  }

  Future<void> loadClients() async {
    final response = await GetIt.instance.get<AdminApiClient>().clients.getClients();

    setState(() {
      clients = response.data;
    });
  }

  Future<void> loadTiers() async {
    final response = await GetIt.I.get<AdminApiClient>().tiers.getTiers();

    tiers = response.data;
  }
}
