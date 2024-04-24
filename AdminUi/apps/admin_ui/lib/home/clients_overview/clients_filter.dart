import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:admin_ui/core/core.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:multi_dropdown/multiselect_dropdown.dart';

class ClientsFilter extends StatefulWidget {
  const ClientsFilter({
    required this.onFilterChanged,
    required this.loadedClients,
    super.key,
  });

  final void Function(List<Clients> filteredClients) onFilterChanged;
  final List<Clients> loadedClients;

  @override
  State<ClientsFilter> createState() => _ClientsFilterState();
}

class _ClientsFilterState extends State<ClientsFilter> {
  late ScrollController _scrollController;
  IdentityOverviewFilter filter = IdentityOverviewFilter();

  late MultiSelectController<dynamic> _tierController;

  late String _enteredClientId;
  late String _enteredDisplayName;
  late List<String> _selectedTiers;
  DateTime? _selectedCreatedAt;
  late String _selectedCreatedAtOperator;
  late String _numberOfIdentitiesOperator;
  late String _numberOfIdentities;

  final operators = <String>['=', '<', '>', '<=', '>='];

  @override
  void initState() {
    super.initState();
    _tierController = MultiSelectController();
    _scrollController = ScrollController();
    _enteredClientId = '';
    _enteredDisplayName = '';
    _selectedTiers = [];
    _selectedCreatedAtOperator = '=';
    _numberOfIdentitiesOperator = '=';
    _numberOfIdentities = '';
    loadTiers();
  }

  @override
  void dispose() {
    _tierController.dispose();
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
              label: 'Address',
              onEnteredText: (String enteredText) {
                _enteredClientId = enteredText;

                sendFilters();
              },
            ),
            Gaps.w16,
            InputField(
              label: 'Display Name',
              onEnteredText: (String enteredText) {
                _enteredDisplayName = enteredText;

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
            NumberFilter(
              operators: operators,
              label: 'Number of Identities',
              onNumberSelected: (String operator, String enteredValue) {
                _numberOfIdentities = enteredValue;
                _numberOfIdentitiesOperator = operator;

                sendFilters();
              },
            ),
            Gaps.w16,
            DateFilter(
              operators: operators,
              label: 'Created At',
              onDateSelected: (DateTime? selectedDate, String operator) {
                setState(() {
                  _selectedCreatedAt = selectedDate;
                  _selectedCreatedAtOperator = operator;
                  sendFilters();
                });
              },
            ),
          ],
        ),
      ),
    );
  }

  void sendFilters() {
    var filteredList = widget.loadedClients;

    if (_enteredClientId.isNotEmpty) {
      filteredList = filteredList.where((client) => client.clientId.contains(_enteredClientId)).toList();
    }

    if (_enteredDisplayName.isNotEmpty) {
      filteredList = filteredList.where((client) => client.displayName.contains(_enteredDisplayName)).toList();
    }

    if (_selectedTiers.isNotEmpty) {
      filteredList = filteredList.where((client) => _selectedTiers.contains(client.defaultTier.id)).toList();
    }

    if (_selectedCreatedAt != null) {
      filteredList = filteredList.where((client) => applyDateFilter(client.createdAt, _selectedCreatedAt!, _selectedCreatedAtOperator)).toList();
    }

    if (_numberOfIdentities.isNotEmpty) {
      filteredList = filteredList
          .where((client) => applyNumberFilter('${client.numberOfIdentities}', _numberOfIdentities, _numberOfIdentitiesOperator))
          .toList();
    }

    widget.onFilterChanged(filteredList);
  }

  bool applyDateFilter(DateTime clientDate, DateTime filterDate, String operator) {
    final clientDateAtMidnight = DateTime(clientDate.year, clientDate.month, clientDate.day);
    final filterDateAtMidnight = DateTime(filterDate.year, filterDate.month, filterDate.day);

    switch (operator) {
      case '=':
        return clientDateAtMidnight.isAtSameMomentAs(filterDateAtMidnight);
      case '<':
        return clientDateAtMidnight.isBefore(filterDateAtMidnight);
      case '>':
        return clientDateAtMidnight.isAfter(filterDateAtMidnight);
      case '<=':
        return clientDateAtMidnight.isBefore(filterDateAtMidnight) || clientDateAtMidnight.isAtSameMomentAs(filterDateAtMidnight);
      case '>=':
        return clientDateAtMidnight.isAfter(filterDateAtMidnight) || clientDateAtMidnight.isAtSameMomentAs(filterDateAtMidnight);
      default:
        throw ArgumentError('Unsupported operator used');
    }
  }

  bool applyNumberFilter(String clientNumber, String filterNumber, String operator) {
    final clientNum = int.tryParse(clientNumber) ?? 0;
    final filterNum = int.tryParse(filterNumber) ?? 0;

    switch (operator) {
      case '=':
        return clientNum == filterNum;
      case '<':
        return clientNum < filterNum;
      case '>':
        return clientNum > filterNum;
      case '<=':
        return clientNum <= filterNum;
      case '>=':
        return clientNum >= filterNum;
      default:
        throw ArgumentError('Unsupported operator used');
    }
  }

  Future<void> loadTiers() async {
    final response = await GetIt.I.get<AdminApiClient>().tiers.getTiers();
    final defaultTiers = response.data.where((element) => element.canBeUsedAsDefaultForClient == true).toList();
    final tierItems = defaultTiers.map((tier) => ValueItem(label: tier.name, value: tier.id)).toList();
    setState(() {
      _tierController.setOptions(tierItems);
    });
  }
}
