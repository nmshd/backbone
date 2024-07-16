import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

import '/core/core.dart';

class DeletionProcessTable extends StatefulWidget {
  final String address;

  const DeletionProcessTable({required this.address, super.key});

  @override
  State<DeletionProcessTable> createState() => _DeletionProcessTableState();
}

class _DeletionProcessTableState extends State<DeletionProcessTable> {
  List<DeletionProcess>? _deletionProcesses;

  @override
  void initState() {
    super.initState();
    _loadIdentityDeletionProcesses();
  }

  @override
  Widget build(BuildContext context) {
    return Theme(
      data: Theme.of(context).copyWith(dividerColor: Colors.transparent),
      child: ExpansionTile(
        title: Text(context.l10n.deletionProcessTable_title),
        subtitle: Text(context.l10n.deletionProcessTable_title_description),
        children: [
          Card(
            child: Column(
              children: [
                if (_deletionProcesses == null)
                  const Center(child: CircularProgressIndicator())
                else
                  SizedBox(
                    width: double.infinity,
                    height: 500,
                    child: DataTable2(
                      empty: Text(context.l10n.deletionProcessTable_noDeletionProcessFound),
                      columns: [
                        DataColumn2(label: Text(context.l10n.id)),
                        DataColumn2(label: Text(context.l10n.deletionProcessTable_status), size: ColumnSize.S),
                        DataColumn2(label: Text(context.l10n.createdAt), size: ColumnSize.S),
                        DataColumn2(label: Text(context.l10n.deletionProcessTable_approvalReminders)),
                        DataColumn2(label: Text(context.l10n.deletionProcessTable_approvedAt)),
                        DataColumn2(label: Text(context.l10n.deletionProcessTable_approvedByDevice)),
                        DataColumn2(label: Text(context.l10n.deletionProcessTable_gracePeriodReminders)),
                        DataColumn2(label: Text(context.l10n.deletionProcessTable_gracePeriodEndsAt)),
                      ],
                      rows: _deletionProcesses!.map(
                        (deletionProcess) {
                          final isDisabled = _isRowDisabled(deletionProcess.status);
                          final textColor = isDisabled ? Colors.grey : Colors.black;

                          return DataRow2(
                            onTap: isDisabled
                                ? null
                                : () {
                                    context.push('/identities/${widget.address}/deletion-process-details/${deletionProcess.id}');
                                  },
                            cells: [
                              DataCell(Text(deletionProcess.id, style: TextStyle(color: textColor))),
                              DataCell(Text(deletionProcess.status, style: TextStyle(color: textColor))),
                              DataCell(
                                Text(
                                  '${DateFormat.yMd(Localizations.localeOf(context).languageCode).format(deletionProcess.createdAt)} ${DateFormat.Hms().format(deletionProcess.createdAt)}',
                                  style: TextStyle(color: textColor),
                                ),
                              ),
                              DataCell(
                                _RemindersCell(
                                  reminders: [
                                    deletionProcess.approvalReminder1SentAt,
                                    deletionProcess.approvalReminder2SentAt,
                                    deletionProcess.approvalReminder3SentAt,
                                  ],
                                  textColor: textColor,
                                ),
                              ),
                              DataCell(
                                Text(
                                  deletionProcess.approvedAt != null
                                      ? '${DateFormat.yMd(Localizations.localeOf(context).languageCode).format(deletionProcess.approvedAt!)} ${DateFormat.Hms().format(deletionProcess.approvedAt!)}'
                                      : '',
                                  style: TextStyle(color: textColor),
                                ),
                              ),
                              DataCell(Text(deletionProcess.approvedByDevice ?? '', style: TextStyle(color: textColor))),
                              DataCell(
                                _RemindersCell(
                                  reminders: [
                                    deletionProcess.gracePeriodReminder1SentAt,
                                    deletionProcess.gracePeriodReminder2SentAt,
                                    deletionProcess.gracePeriodReminder3SentAt,
                                  ],
                                  textColor: textColor,
                                ),
                              ),
                              DataCell(
                                Text(
                                  deletionProcess.approvedAt != null
                                      ? '${DateFormat.yMd(Localizations.localeOf(context).languageCode).format(deletionProcess.gracePeriodEndsAt!)} ${DateFormat.Hms().format(deletionProcess.gracePeriodEndsAt!)}'
                                      : '',
                                  style: TextStyle(color: textColor),
                                ),
                              ),
                            ],
                          );
                        },
                      ).toList(),
                    ),
                  ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  bool _isRowDisabled(String deletionProcessStatus) {
    return deletionProcessStatus == 'Rejected' || deletionProcessStatus == 'Cancelled';
  }

  Future<void> _loadIdentityDeletionProcesses() async {
    final deletionProcesses = await GetIt.I.get<AdminApiClient>().deletionProcesses.getIdentityDeletionProcesses(address: widget.address);
    if (mounted) {
      setState(() {
        _deletionProcesses = deletionProcesses.data;
        _deletionProcesses!.sort((a, b) => b.createdAt.compareTo(a.createdAt));
      });
    }
  }
}

class _RemindersCell extends StatelessWidget {
  final List<DateTime?> reminders;
  final Color textColor;

  const _RemindersCell({
    required this.reminders,
    required this.textColor,
  });

  @override
  Widget build(BuildContext context) {
    final validReminders = reminders.where((date) => date != null).toList();

    if (validReminders.isEmpty) {
      return Text(context.l10n.deletionProcessTable_approvalRemindersCell_noData, style: TextStyle(color: textColor));
    }

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: validReminders.asMap().entries.map((entry) {
        final index = entry.key + 1;
        final date = entry.value!;
        final color = _isDatePassed(date) ? Colors.green : textColor;
        return Padding(
          padding: const EdgeInsets.only(bottom: 4),
          child: Text(
            '${context.l10n.deletionProcessTable_approvalRemindersCell_reminder} $index: ${DateFormat.yMd(Localizations.localeOf(context).languageCode).format(date)} ${DateFormat.Hms().format(date)}',
            style: TextStyle(color: color),
          ),
        );
      }).toList(),
    );
  }

  bool _isDatePassed(DateTime date) {
    return date.isBefore(DateTime.now());
  }
}
