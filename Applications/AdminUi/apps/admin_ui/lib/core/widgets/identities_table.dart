import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '/core/core.dart';

class IdentitiesTable extends StatefulWidget {
  final Client? clientDetails;
  final TierDetails? tierDetails;

  const IdentitiesTable({this.clientDetails, this.tierDetails, super.key})
    : assert(clientDetails != null || tierDetails != null, 'Either client details or tier details must be provided'),
      assert(clientDetails == null || tierDetails == null, 'Only one of client details or tier details can be provided');

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
      _dataSource = IdentityDataTableSource(
        locale: locale,
        hideClientColumn: true,
        filter: IdentityOverviewFilter(clients: [widget.clientDetails!.clientId]),
        navigateToIdentity: ({required String address}) => context.push('/identities/$address'),
      );
    } else {
      _dataSource = IdentityDataTableSource(
        locale: locale,
        hideTierColumn: true,
        filter: IdentityOverviewFilter(tiers: [widget.tierDetails!.id]),
        navigateToIdentity: ({required String address}) => context.push('/identities/$address'),
      );
    }
  }

  @override
  void dispose() {
    _dataSource.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Theme(
      data: Theme.of(context).copyWith(dividerColor: Colors.transparent),
      child: ExpansionTile(
        title: Text(context.l10n.identities),
        subtitle: Text(
          widget.clientDetails != null ? context.l10n.identityList_client_titleDescription : context.l10n.identityList_tier_titleDescription,
        ),
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
}
