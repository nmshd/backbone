import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:enmeshed_ui_kit/enmeshed_ui_kit.dart';
import 'package:flutter/material.dart';

import '/core/core.dart';
import 'identity_messages_data_table_source.dart';

class IdentityMessagesTable extends StatefulWidget {
  final IdentityMessagesDataTableSource dataSource;
  final MessageType type;
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
                wrapInCard: false,
                empty: Text(widget.emptyTableMessage),
                errorBuilder: (error) => Center(
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Text(context.l10n.identityMessageTable_errorLoadingData),
                      Gaps.h16,
                      FilledButton(onPressed: widget.dataSource.refreshDatasource, child: Text(context.l10n.retry)),
                    ],
                  ),
                ),
                columns: <DataColumn2>[
                  if (widget.type == MessageType.outgoing) DataColumn2(label: Text(context.l10n.identityMessageTable_recipients), size: ColumnSize.L),
                  if (widget.type == MessageType.incoming) ...[
                    DataColumn2(label: Text(context.l10n.identityMessageTable_senderAddress)),
                    DataColumn2(label: Text(context.l10n.identityMessageTable_senderDevice), size: ColumnSize.S),
                  ],
                  DataColumn2(label: Text(context.l10n.identityMessageTable_numberOfAttachments)),
                  DataColumn2(label: Text(context.l10n.identityMessageTable_sendDate), size: ColumnSize.S),
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
