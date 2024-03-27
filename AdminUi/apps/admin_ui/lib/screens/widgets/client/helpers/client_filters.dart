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

  final void Function(ClientOverviewFilter clientFilter) filterClients;

  @override
  State<ClientFilter> createState() => _ClientFilterState();
}

class _ClientFilterState extends State<ClientFilter> {
  List<Tier> tiers = [];
  ClientOverviewFilter filter = ClientOverviewFilter();

  late bool isCreatedAtSelected;
  late bool isLastLoginAtSelected;

  final operators = <String>['=', '<', '>', '<=', '>='];

  @override
  void initState() {
    super.initState();
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
          onEnteredText: (String enteredClientID) {
            setState(() {
              if (enteredClientID.isNotEmpty) {
                filter.clientID = enteredClientID;
                setFilter();
              }
            });
          },
        ),
        const SizedBox(
          width: 8,
        ),
        InputField(
          title: 'Display Name',
          onEnteredText: (String enteredDisplayName) {
            setState(() {
              if (enteredDisplayName.isNotEmpty) {
                filter.displayName = enteredDisplayName;
                setFilter();
              }
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
          onSelectedValues: (List<String> selectedTiers) {
            setState(() {
              if (selectedTiers.isNotEmpty) {
                filter.tiers = selectedTiers;
                setFilter();
              }
            });
          },
        ),
        const SizedBox(
          width: 8,
        ),
        NumberFilter(
          operators: operators,
          onNumberSelected: (String operator, String enteredNumberOfIdentities) {
            setState(() {
              if (enteredNumberOfIdentities.isNotEmpty) {
                filter
                  ..numberOfIdentitiesOperator = enteredNumberOfIdentities
                  ..numberOfIdentities = operator;
              }
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
              isCreatedAtSelected = isDateSelected;
              if (selectedDate.toString().isNotEmpty && isCreatedAtSelected) {
                filter
                  ..createdAt = selectedDate.toString().substring(0, 10)
                  ..createdAtOperator = operator;

                setFilter();
              }
            });
          },
        ),
      ],
    );
  }

  void setFilter() {
    setState(() {
      widget.filterClients(filter);
    });
  }

  Future<void> loadTiers() async {
    final response = await GetIt.I.get<AdminApiClient>().tiers.getTiers();

    tiers = response.data;
  }
}

class ClientOverviewFilter {
  String? clientID;
  String? displayName;
  List<String>? tiers;
  String? createdAt;
  String? createdAtOperator;
  String? numberOfIdentitiesOperator;
  String? numberOfIdentities;

  ClientOverviewFilter({
    this.clientID,
    this.displayName,
    this.tiers,
    this.numberOfIdentities,
    this.numberOfIdentitiesOperator,
    this.createdAt,
    this.createdAtOperator,
  });
}
