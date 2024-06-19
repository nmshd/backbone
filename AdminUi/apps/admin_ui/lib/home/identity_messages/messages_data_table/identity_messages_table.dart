import 'package:admin_ui/core/extensions.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';

import '../../../../core/constants.dart';
import 'identity_messages_table_source.dart';

export 'identity_messages_table_source.dart';

class IdentityMessagesTable extends StatefulWidget {
  final IdentityMessagesTableSource dataSource;
  final String type;
  final String title;
  final String subtitle;
  final String emptyTableMessage;

  const IdentityMessagesTable({
    required this.dataSource,
    required this.type,
    required this.title,
    required this.subtitle,
    required this.emptyTableMessage,
    super.key,
  });

  @override
  State<IdentityMessagesTable> createState() => _IdentityMessagesTableState();
}

class _IdentityMessagesTableState extends State<IdentityMessagesTable> {
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
                  if (widget.type == 'Outgoing') DataColumn2(label: Text(context.l10n.recipients), size: ColumnSize.L),
                  if (widget.type == 'Incoming') DataColumn2(label: Text(context.l10n.senderAddress)),
                  if (widget.type == 'Incoming') DataColumn2(label: Text(context.l10n.senderDevice), size: ColumnSize.S),
                  DataColumn2(label: Text(context.l10n.sendDate), size: ColumnSize.S),
                  DataColumn2(label: Text(context.l10n.numberOfAttachments)),
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
