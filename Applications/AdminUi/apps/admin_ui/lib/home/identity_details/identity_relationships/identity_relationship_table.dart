import 'package:admin_ui/core/extensions.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:enmeshed_ui_kit/enmeshed_ui_kit.dart';
import 'package:flutter/material.dart';

import 'identity_relationship_data_table_source.dart';

class IdentityRelationshipTable extends StatefulWidget {
  final IdentityRelationshipDataTableSource dataSource;

  const IdentityRelationshipTable({required this.dataSource, super.key});

  @override
  State<IdentityRelationshipTable> createState() => _IdentityRelationshipTableState();
}

class _IdentityRelationshipTableState extends State<IdentityRelationshipTable> {
  int _rowsPerPage = 5;

  @override
  Widget build(BuildContext context) {
    return Theme(
      data: Theme.of(context).copyWith(dividerColor: Colors.transparent),
      child: ExpansionTile(
        title: Text(context.l10n.identityRelationshipTable_relationships),
        subtitle: Text(context.l10n.identityRelationshipTable_viewIdentityRelationships),
        children: [
          Card(
            child: SizedBox(
              width: double.infinity,
              height: 500,
              child: AsyncPaginatedDataTable2(
                rowsPerPage: _rowsPerPage,
                onRowsPerPageChanged: _setRowsPerPage,
                showFirstLastButtons: true,
                columnSpacing: 5,
                source: widget.dataSource,
                isVerticalScrollBarVisible: true,
                renderEmptyRowsInTheEnd: false,
                availableRowsPerPage: const [5, 10, 25, 50, 100],
                wrapInCard: false,
                empty: Text(context.l10n.identityRelationshipTable_emptyRelationshipTable),
                errorBuilder:
                    (error) => Center(
                      child: Column(
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          Text(context.l10n.identityRelationshipTable_errorLoadingData),
                          Gaps.h16,
                          FilledButton(onPressed: widget.dataSource.refreshDatasource, child: Text(context.l10n.retry)),
                        ],
                      ),
                    ),
                columns: <DataColumn2>[
                  DataColumn2(label: Text(context.l10n.identityRelationshipTable_peer), size: ColumnSize.L),
                  DataColumn2(label: Text(context.l10n.identityRelationshipTable_requestedBy), size: ColumnSize.S),
                  DataColumn2(label: Text(context.l10n.identityRelationshipTable_templateId), size: ColumnSize.S),
                  DataColumn2(label: Text(context.l10n.identityRelationshipTable_status), size: ColumnSize.S),
                  DataColumn2(label: Text(context.l10n.identityRelationshipTable_creationDate), size: ColumnSize.S),
                  DataColumn2(label: Text(context.l10n.identityRelationshipTable_answeredAt), size: ColumnSize.S),
                  DataColumn2(label: Text(context.l10n.identityRelationshipTable_createdByDevice), size: ColumnSize.S),
                  DataColumn2(label: Text(context.l10n.identityRelationshipTable_answeredByDevice), size: ColumnSize.S),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  void _setRowsPerPage(int? newValue) {
    _rowsPerPage = newValue ?? _rowsPerPage;
    widget.dataSource.refreshDatasource();
  }
}
