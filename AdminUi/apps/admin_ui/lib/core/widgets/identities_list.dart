import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '/core/core.dart';

class IdentitiesList extends StatefulWidget {
  final Client? clientDetails;
  final TierDetails? tierDetails;

  const IdentitiesList({this.clientDetails, this.tierDetails, super.key});

  @override
  State<IdentitiesList> createState() => _IdentitiesListState();
}

class _IdentitiesListState extends State<IdentitiesList> {
  late IdentityDataTableSource _dataSource;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();

    final locale = Localizations.localeOf(context);

    if (widget.clientDetails != null) {
      _dataSource = IdentityDataTableSource(
        locale: locale,
        hideClientColumn: true,
        navigateToIdentity: ({required String address}) {
          context.push('/identities/$address');
        },
        filter: IdentityOverviewFilter(clients: [widget.clientDetails!.clientId]),
      );
    } else if (widget.tierDetails != null) {
      _dataSource = IdentityDataTableSource(
        locale: locale,
        hideTierColumn: true,
        navigateToIdentity: ({required String address}) {
          context.push('/identities/$address');
        },
        filter: IdentityOverviewFilter(tiers: [widget.tierDetails!.id]),
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
}
