import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '/core/core.dart';

enum FilterType {
  clientFilter,
  tierFilter,
}

class IdentitiesTable extends StatefulWidget {
  final Client? clientDetails;
  final TierDetails? tierDetails;

  const IdentitiesTable({this.clientDetails, this.tierDetails, super.key});

  @override
  State<IdentitiesTable> createState() => _IdentitiesTableState();
}

class _IdentitiesTableState extends State<IdentitiesTable> {
  late IdentityDataTableSource _dataSource;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();

    final locale = Localizations.localeOf(context);

    if (widget.clientDetails != null) {
      _dataSource = _fillDataTableSource(filterType: FilterType.clientFilter, locale: locale, hideViewSpecificColumn: true);
    } else if (widget.tierDetails != null) {
      _dataSource = _fillDataTableSource(filterType: FilterType.tierFilter, locale: locale, hideViewSpecificColumn: true);
    }
  }

  @override
  void dispose() {
    _dataSource.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    var description = '';
    if (widget.clientDetails != null) {
      description = context.l10n.identityList_client_titleDescription;
    } else if (widget.tierDetails != null) {
      description = context.l10n.identityList_tier_titleDescription;
    }

    return Theme(
      data: Theme.of(context).copyWith(dividerColor: Colors.transparent),
      child: ExpansionTile(
        title: Text(context.l10n.identities),
        subtitle: Text(description),
        children: [
          Card(
            child: Column(
              children: [
                IdentitiesFilter(
                  fixedClientId: widget.clientDetails?.clientId,
                  fixedTierId: widget.tierDetails?.id,
                  onFilterChanged: ({IdentityOverviewFilter? filter}) async {
                    _dataSource
                      ..filter = filter
                      ..refreshDatasource();
                  },
                ),
                SizedBox(
                  height: 500,
                  child: IdentitiesDataTable(
                    dataSource: _dataSource,
                    hideClientColumn: widget.clientDetails != null,
                    hideTierColumn: widget.tierDetails != null,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  IdentityDataTableSource _fillDataTableSource({required FilterType filterType, required Locale locale, required bool hideViewSpecificColumn}) {
    final hideClientColumn = filterType == FilterType.clientFilter && hideViewSpecificColumn;
    final hideTierColumn = filterType == FilterType.tierFilter && hideViewSpecificColumn;
    final localFilter = switch (filterType) {
      FilterType.clientFilter => widget.clientDetails != null ? IdentityOverviewFilter(clients: [widget.clientDetails!.clientId]) : null,
      FilterType.tierFilter => widget.tierDetails != null ? IdentityOverviewFilter(tiers: [widget.tierDetails!.id]) : null,
    };

    return IdentityDataTableSource(
      locale: locale,
      hideClientColumn: hideClientColumn,
      hideTierColumn: hideTierColumn,
      filter: localFilter,
      navigateToIdentity: ({required String address}) {
        context.push('/identities/$address');
      },
    );
  }
}
