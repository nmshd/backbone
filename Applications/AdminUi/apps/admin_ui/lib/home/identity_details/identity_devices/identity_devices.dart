import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

import '/core/core.dart';

class IdentityDevices extends StatelessWidget {
  final List<IdentityDevice> devices;

  const IdentityDevices({required this.devices, super.key});

  @override
  Widget build(BuildContext context) {
    return Theme(
      data: Theme.of(context).copyWith(dividerColor: Colors.transparent),
      child: ExpansionTile(
        title: Text(context.l10n.identityDevices_title),
        subtitle: Text(context.l10n.identityDevices_title_description),
        children: [
          Card(
            child: Column(
              children: [
                SizedBox(
                  width: double.infinity,
                  height: 500,
                  child: DataTable2(
                    empty: Text(context.l10n.deletionProcessTable_noDeletionProcessFound),
                    columns: [
                      DataColumn2(label: Text(context.l10n.id)),
                      DataColumn2(label: Text(context.l10n.identityDevices_username)),
                      DataColumn2(label: Text(context.l10n.createdAt)),
                      DataColumn2(label: Text(context.l10n.lastLoginAt)),
                      DataColumn2(label: Text(context.l10n.identityDevices_communicationLanguage)),
                    ],
                    rows: devices.map((device) {
                      final textColor = Theme.of(context).colorScheme.onSecondaryContainer;

                      return DataRow2(
                        cells: [
                          DataCell(Text(device.id, style: TextStyle(color: textColor))),
                          DataCell(Text(device.username, style: TextStyle(color: textColor))),
                          DataCell(
                            Text(
                              DateFormat.yMd(Localizations.localeOf(context).languageCode).format(device.createdAt),
                              style: TextStyle(color: textColor),
                            ),
                          ),
                          DataCell(
                            Text(
                              device.lastLogin != null
                                  ? DateFormat.yMd(Localizations.localeOf(context).languageCode).format(device.lastLogin!.time)
                                  : '-',
                              style: TextStyle(color: textColor),
                            ),
                          ),
                          DataCell(Text(device.communicationLanguage, style: TextStyle(color: textColor))),
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
}
