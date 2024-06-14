import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';

import '../../constants.dart';
import 'identity_messages_table_source.dart';

export 'identity_messages_table_source.dart';

class IdentityMessagesTable extends StatefulWidget {
  final IdentityMessagesTableSource dataSource;
  final bool hideTierColumn;

  const IdentityMessagesTable({required this.dataSource, this.hideTierColumn = false, super.key});

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
        title: const Text('Sent Messages'),
        subtitle: const Text('View sent messages sent by identity.'),
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
                empty: const Text('No sent messages found.'),
                errorBuilder: (error) => Center(
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      const Text('An error occurred loading the data.'),
                      Gaps.h16,
                      FilledButton(onPressed: widget.dataSource.refreshDatasource, child: const Text('Retry')),
                    ],
                  ),
                ),
                columns: <DataColumn2>[
                  const DataColumn2(label: Text('Recipients'), size: ColumnSize.L),
                  if (!widget.hideTierColumn) const DataColumn2(label: Text('Send Date'), size: ColumnSize.S),
                  const DataColumn2(label: Text('Number of Attachments')),
                  const DataColumn2(label: Text('Sender Address')),
                  const DataColumn2(label: Text('Sender Device'), size: ColumnSize.S),
                  const DataColumn2(label: Text('Send Date'), size: ColumnSize.S),
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
