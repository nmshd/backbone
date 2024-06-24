import 'package:admin_ui/core/extensions.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';

import '../../../core/constants.dart';
import 'identity_relationship_source.dart';

export 'identity_relationship_source.dart';

class IdentityRelationshipTable extends StatefulWidget {
  final IdentityRelationshipSource dataSource;
  final String title;
  final String subtitle;
  final String emptyTableMessage;

  const IdentityRelationshipTable({
    required this.dataSource,
    required this.title,
    required this.subtitle,
    required this.emptyTableMessage,
    super.key,
  });

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
        title: Text(widget.title),
        subtitle: Text(widget.subtitle),
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
                empty: Text(widget.emptyTableMessage),
                errorBuilder: (error) => Center(
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Text(context.l10n.error_loading_data),
                      Gaps.h16,
                      FilledButton(onPressed: widget.dataSource.refreshDatasource, child: Text(context.l10n.retry)),
                    ],
                  ),
                ),
                columns: <DataColumn2>[
                  DataColumn2(label: Text(context.l10n.peer), size: ColumnSize.L),
                  DataColumn2(label: Text(context.l10n.requestedBy), size: ColumnSize.S),
                  DataColumn2(label: Text(context.l10n.templateId), size: ColumnSize.S),
                  DataColumn2(label: Text(context.l10n.status), size: ColumnSize.S),
                  DataColumn2(label: Text(context.l10n.creationDate), size: ColumnSize.S),
                  DataColumn2(label: Text(context.l10n.answeredAt), size: ColumnSize.S),
                  DataColumn2(label: Text(context.l10n.createdByDevice), size: ColumnSize.S),
                  DataColumn2(label: Text(context.l10n.answeredByDevice), size: ColumnSize.S),
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
