import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:intl/intl.dart';
import 'package:logger/logger.dart';

import '/core/constants.dart';

class IdentitySentMessagesOverview extends StatefulWidget {
  final String address;
  final String type;
  const IdentitySentMessagesOverview({required this.address, required this.type, super.key});

  @override
  State<IdentitySentMessagesOverview> createState() => _IdentitySentMessagesOverviewState();
}

class _IdentitySentMessagesOverviewState extends State<IdentitySentMessagesOverview> {
  late IdentitySentMessagesTableSource _dataSource;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();

    _dataSource = IdentitySentMessagesTableSource(
      address: widget.address,
      type: widget.type,
      locale: Localizations.localeOf(context),
    );
  }

  @override
  void dispose() {
    _dataSource.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return IdentitySentMessagesTable(dataSource: _dataSource);
  }
}

class IdentitySentMessagesTable extends StatefulWidget {
  final IdentitySentMessagesTableSource dataSource;
  final bool hideTierColumn;

  const IdentitySentMessagesTable({required this.dataSource, this.hideTierColumn = false, super.key});

  @override
  State<IdentitySentMessagesTable> createState() => _IdentitySentMessagesTableState();
}

class _IdentitySentMessagesTableState extends State<IdentitySentMessagesTable> {
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
          )
        ],
      ),
    );
  }

  void _setRowsPerPage(int? newValue) {
    _rowsPerPage = newValue ?? _rowsPerPage;
    widget.dataSource.refreshDatasource();
  }
}

class IdentitySentMessagesTableSource extends AsyncDataTableSource {
  Pagination? _pagination;

  final Locale locale;
  final bool hideTierColumn;
  final String address;
  final String type;

  IdentitySentMessagesTableSource({
    required this.address,
    required this.type,
    required this.locale,
    this.hideTierColumn = false,
  });

  @override
  bool get isRowCountApproximate => false;
  @override
  int get rowCount => _pagination?.totalRecords ?? 0;
  @override
  int get selectedRowCount => 0;
  @override
  Future<AsyncRowsResponse> getRows(int startIndex, int count) async {
    final pageNumber = startIndex ~/ count;
    try {
      final response = await GetIt.I.get<AdminApiClient>().messages.getMessagesByParticipantAddress(
            address: address,
            type: type,
            pageNumber: pageNumber,
            pageSize: count,
          );
      _pagination = response.pagination;
      print(_pagination);

      final rows = response.data.indexed
          .map(
            (message) => DataRow2.byIndex(
              index: pageNumber * count + message.$1,
              cells: [
                DataCell(Text(message.$2.recipients.map((recipient) => recipient.address).join(', '))),
                DataCell(Text(message.$2.senderAddress)),
                DataCell(Text(message.$2.senderDevice)),
                DataCell(Text(message.$2.numberOfAttachments.toString())),
                DataCell(
                  Tooltip(
                    message: '${DateFormat.yMd(locale.languageCode).format(message.$2.sendDate)} ${DateFormat.Hms().format(message.$2.sendDate)}',
                    child: Text(DateFormat.yMd(locale.languageCode).format(message.$2.sendDate)),
                  ),
                ),
              ],
            ),
          )
          .toList();
      return AsyncRowsResponse(response.pagination.totalRecords, rows);
    } catch (e) {
      GetIt.I.get<Logger>().e('Failed to load data: $e');
      throw Exception('Failed to load data: $e');
    }
  }
}
