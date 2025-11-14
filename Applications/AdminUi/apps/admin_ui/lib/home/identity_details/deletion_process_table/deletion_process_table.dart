import 'dart:async';

import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:collection/collection.dart';
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

    unawaited(_reloadIdentityDeletionProcesses());
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
                        DataColumn2(label: Text(context.l10n.deletionProcessTable_status), size: .S),
                        DataColumn2(label: Text(context.l10n.createdAt), size: .S),
                        DataColumn2(label: Text(context.l10n.deletionProcessTable_createdByDevice)),
                        DataColumn2(label: Text(context.l10n.deletionProcessTable_gracePeriodReminders), size: .L),
                        DataColumn2(label: Text(context.l10n.deletionProcessTable_gracePeriodEndsAt), size: .S),
                      ],
                      rows: _deletionProcesses!.map((deletionProcess) {
                        final textColor = Theme.of(context).colorScheme.onSecondaryContainer;

                        return DataRow2(
                          specificRowHeight: 60,
                          onTap: () async {
                            final result = await context.push<bool?>('/identities/${widget.address}/deletion-process-details/${deletionProcess.id}');
                            if (result != true) return;

                            await _reloadIdentityDeletionProcesses();
                          },
                          cells: [
                            DataCell(Text(deletionProcess.id, style: TextStyle(color: textColor))),
                            DataCell(
                              Text(switch (deletionProcess.status) {
                                .active => context.l10n.deletionProcessDetails_status_active,
                                .cancelled => context.l10n.deletionProcessDetails_status_cancelled,
                                .deleting => context.l10n.deletionProcessDetails_status_deleting,
                              }, style: TextStyle(color: textColor)),
                            ),
                            DataCell(
                              Text(
                                DateFormat.yMd(Localizations.localeOf(context).languageCode).format(deletionProcess.createdAt),
                                style: TextStyle(color: textColor),
                              ),
                            ),
                            DataCell(Text(deletionProcess.createdByDevice ?? '', style: TextStyle(color: textColor))),
                            DataCell(
                              _RemindersCell(
                                name: context.l10n.deletionProcessTable_gracePeriodRemindersCell_reminder,
                                noDataText: context.l10n.deletionProcessTable_gracePeriodRemindersCell_noData,
                                reminders: deletionProcess.gracePeriodReminders,
                                textColor: textColor,
                              ),
                            ),
                            DataCell(
                              Text(
                                deletionProcess.gracePeriodEndsAt != null
                                    ? DateFormat.yMd(Localizations.localeOf(context).languageCode).format(deletionProcess.gracePeriodEndsAt!)
                                    : '',
                                style: TextStyle(color: textColor),
                              ),
                            ),
                          ],
                        );
                      }).toList(),
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
}

class _RemindersCell extends StatelessWidget {
  final String name;
  final String noDataText;
  final List<DateTime> reminders;
  final Color textColor;

  const _RemindersCell({required this.name, required this.noDataText, required this.reminders, required this.textColor});

  @override
  Widget build(BuildContext context) {
    if (reminders.isEmpty) return Text(noDataText, style: TextStyle(color: textColor));

    return Column(
      mainAxisAlignment: .center,
      crossAxisAlignment: .start,
      children: reminders.mapIndexed((index, date) {
        return Text(
          '$name ${index + 1}: ${DateFormat.yMd(Localizations.localeOf(context).languageCode).format(date)} ${DateFormat.Hms().format(date)}',
          style: TextStyle(color: textColor),
        );
      }).toList(),
    );
  }
}

extension _Reminders on IdentityDeletionProcess {
  List<DateTime> get gracePeriodReminders => [?gracePeriodReminder1SentAt, ?gracePeriodReminder2SentAt, ?gracePeriodReminder3SentAt];
}
