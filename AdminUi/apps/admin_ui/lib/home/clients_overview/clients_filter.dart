import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:admin_ui/core/core.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:multi_dropdown/multiselect_dropdown.dart';

class ClientsFilter {
  final String? clientId;
  final String? displayName;
  final List<String>? tiers;
  final (FilterOperator, DateTime)? createdAt;
  final (FilterOperator, int)? numberOfIdentities;

  const ClientsFilter({
    this.clientId,
    this.displayName,
    this.tiers,
    this.createdAt,
    this.numberOfIdentities,
  });

  static const empty = ClientsFilter();

  ClientsFilter copyWith({
    Optional<String>? clientId,
    Optional<String>? displayName,
    Optional<List<String>>? tiers,
    Optional<(FilterOperator, DateTime)>? createdAt,
    Optional<(FilterOperator, int)>? numberOfIdentities,
  }) {
    return ClientsFilter(
      clientId: clientId != null ? clientId.value : this.clientId,
      displayName: displayName != null ? displayName.value : this.displayName,
      tiers: tiers != null ? tiers.value : this.tiers,
      createdAt: createdAt != null ? createdAt.value : this.createdAt,
      numberOfIdentities: numberOfIdentities != null ? numberOfIdentities.value : this.numberOfIdentities,
    );
  }

  List<Clients> apply(List<Clients> clients) => clients.where(matches).toList();

  bool matches(Clients client) {
    if (clientId != null && !client.clientId.contains(clientId!)) return false;
    if (displayName != null && !client.displayName.contains(displayName!)) return false;
    if (tiers != null && !tiers!.contains(client.defaultTier.id)) return false;
    if (createdAt != null && !_applyDateFilter(client.createdAt, createdAt!.$2, createdAt!.$1)) return false;
    if (numberOfIdentities != null && !_applyNumberFilter(client.numberOfIdentities ?? 0, numberOfIdentities!.$2, numberOfIdentities!.$1)) {
      return false;
    }

    return true;
  }

  bool _applyDateFilter(DateTime clientDate, DateTime filterDate, FilterOperator filterOperator) {
    final clientDateAtMidnight = DateTime(clientDate.year, clientDate.month, clientDate.day);
    final filterDateAtMidnight = DateTime(filterDate.year, filterDate.month, filterDate.day);

    return switch (filterOperator) {
      FilterOperator.equal => clientDateAtMidnight.isAtSameMomentAs(filterDateAtMidnight),
      FilterOperator.lessThan => clientDateAtMidnight.isBefore(filterDateAtMidnight),
      FilterOperator.greaterThan => clientDateAtMidnight.isAfter(filterDateAtMidnight),
      FilterOperator.lessThanOrEqual =>
        clientDateAtMidnight.isBefore(filterDateAtMidnight) || clientDateAtMidnight.isAtSameMomentAs(filterDateAtMidnight),
      FilterOperator.greaterThanOrEqual =>
        clientDateAtMidnight.isAfter(filterDateAtMidnight) || clientDateAtMidnight.isAtSameMomentAs(filterDateAtMidnight),
      FilterOperator.notEqual => !clientDateAtMidnight.isAtSameMomentAs(filterDateAtMidnight),
    };
  }

  bool _applyNumberFilter(int clientNumber, int filterNumber, FilterOperator filterOperator) {
    return switch (filterOperator) {
      FilterOperator.equal => clientNumber == filterNumber,
      FilterOperator.lessThan => clientNumber < filterNumber,
      FilterOperator.greaterThan => clientNumber > filterNumber,
      FilterOperator.lessThanOrEqual => clientNumber <= filterNumber,
      FilterOperator.greaterThanOrEqual => clientNumber >= filterNumber,
      FilterOperator.notEqual => clientNumber != filterNumber,
    };
  }
}

class ClientsFilterRow extends StatefulWidget {
  final void Function(ClientsFilter filter) onFilterChanged;

  const ClientsFilterRow({
    required this.onFilterChanged,
    super.key,
  });

  @override
  State<ClientsFilterRow> createState() => _ClientsFilterRowState();
}

class _ClientsFilterRowState extends State<ClientsFilterRow> {
  late MultiSelectController<String> _tierController;

  ClientsFilter filter = ClientsFilter.empty;

  @override
  void initState() {
    super.initState();

    _tierController = MultiSelectController();
    _loadTiers();
  }

  @override
  void dispose() {
    _tierController.dispose();

    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      scrollDirection: Axis.horizontal,
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 8),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            InputField(
              label: 'Client ID',
              onEnteredText: (String enteredText) {
                filter = filter.copyWith(clientId: enteredText.isEmpty ? const Optional.absent() : Optional(enteredText));

                widget.onFilterChanged(filter);
              },
            ),
            Gaps.w16,
            InputField(
              label: 'Display Name',
              onEnteredText: (String enteredText) {
                filter = filter.copyWith(displayName: enteredText.isEmpty ? const Optional.absent() : Optional(enteredText));

                widget.onFilterChanged(filter);
              },
            ),
            Gaps.w16,
            MultiSelectFilter(
              label: 'Default Tier',
              searchLabel: 'Search Tiers',
              controller: _tierController,
              onOptionSelected: (List<ValueItem<dynamic>> selectedOptions) {
                filter = filter.copyWith(
                  tiers: selectedOptions.isEmpty ? const Optional.absent() : Optional(selectedOptions.map((item) => item.value as String).toList()),
                );

                widget.onFilterChanged(filter);
              },
            ),
            Gaps.w16,
            NumberFilter(
              label: 'Number of Identitites',
              onNumberSelected: (FilterOperator operator, String enteredValue) {
                filter = filter.copyWith(
                  numberOfIdentities: enteredValue.isEmpty ? const Optional.absent() : Optional((operator, int.parse(enteredValue))),
                );

                widget.onFilterChanged(filter);
              },
            ),
            Gaps.w16,
            DateFilter(
              label: 'Created At',
              onFilterSelected: (FilterOperator operator, DateTime? selectedDate) {
                filter = filter.copyWith(
                  createdAt: selectedDate == null ? const Optional.absent() : Optional((operator, selectedDate)),
                );

                widget.onFilterChanged(filter);
              },
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _loadTiers() async {
    final response = await GetIt.I.get<AdminApiClient>().tiers.getTiers();
    final defaultTiers = response.data.where((element) => element.canBeUsedAsDefaultForClient == true).toList();
    final tierItems = defaultTiers.map((tier) => ValueItem(label: tier.name, value: tier.id)).toList();
    _tierController.setOptions(tierItems);
  }
}
