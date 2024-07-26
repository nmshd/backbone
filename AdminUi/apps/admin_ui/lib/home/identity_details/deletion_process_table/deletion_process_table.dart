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
  List<IdentityDeletionProcess>? _deletionProcesses;

  @override
  void initState() {
    super.initState();
    _reloadIdentityDeletionProcesses();
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
                        DataColumn2(label: Text(context.l10n.deletionProcessTable_approvalReminders), size: ColumnSize.L),
                        DataColumn2(label: Text(context.l10n.deletionProcessTable_approvedAt), size: ColumnSize.S),
                        DataColumn2(label: Text(context.l10n.deletionProcessTable_approvedByDevice)),
                        DataColumn2(label: Text(context.l10n.deletionProcessTable_gracePeriodReminders), size: ColumnSize.L),
                        DataColumn2(label: Text(context.l10n.deletionProcessTable_gracePeriodEndsAt), size: ColumnSize.S),
                      ],
                      rows: _deletionProcesses!.map(
                        (deletionProcess) {
                          final isDisabled = _isRowDisabled(deletionProcess.status);
                          final textColor = isDisabled ? Colors.grey : Theme.of(context).colorScheme.onSecondaryContainer;

                          return DataRow2(
                            specificRowHeight: 60,
                            onTap: isDisabled
                                ? null
                                : () async {
                                    final result =
                                        await context.push<bool?>('/identities/${widget.address}/deletion-process-details/${deletionProcess.id}');
                                    if (result! == true) {
                                      await _reloadIdentityDeletionProcesses();
                                    }
                                  },
                            cells: [
                              DataCell(Text(deletionProcess.id, style: TextStyle(color: textColor))),
                              DataCell(
                                Text(
                                  deletionProcess.status.name == 'WaitingForApproval' ? 'Waiting for Approval' : deletionProcess.status.name,
                                  style: TextStyle(color: textColor),
                                ),
                              ),
                              DataCell(
                                Text(
                                  DateFormat.yMd(Localizations.localeOf(context).languageCode).format(deletionProcess.createdAt),
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
                                      ? '${DateFormat.yMd(Localizations.localeOf(context).languageCode).format(deletionProcess.approvedAt!)} '
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
                                      ? DateFormat.yMd(Localizations.localeOf(context).languageCode).format(deletionProcess.gracePeriodEndsAt!)
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

  Future<void> _reloadIdentityDeletionProcesses() async {
    final deletionProcesses = await GetIt.I.get<AdminApiClient>().identities.getIdentityDeletionProcesses(address: widget.address);
    if (mounted) {
      setState(() {
        _deletionProcesses = deletionProcesses.data;
        _deletionProcesses!.sort((a, b) => b.createdAt.compareTo(a.createdAt));
      });
    }
  }

  bool _isRowDisabled(DeletionProcessStatus deletionProcessStatus) {
    return deletionProcessStatus.name == 'Rejected' || deletionProcessStatus.name == 'Cancelled';
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
      mainAxisAlignment: MainAxisAlignment.center,
      crossAxisAlignment: CrossAxisAlignment.start,
      children: validReminders.asMap().entries.map((entry) {
        final index = entry.key + 1;
        final date = entry.value!;
        final color = textColor;
        return Text(
          '${context.l10n.deletionProcessTable_approvalRemindersCell_reminder} $index: ${DateFormat.yMd(Localizations.localeOf(context).languageCode).format(date)} ${DateFormat.Hms().format(date)}',
          style: TextStyle(color: color),
        );
      }).toList(),
    );
  }
}
