import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

import '/core/core.dart';
import 'modals/create_announcement_dialog.dart';

class AnnouncementsOverview extends StatefulWidget {
  const AnnouncementsOverview({super.key});

  @override
  State<AnnouncementsOverview> createState() => _AnnouncementsOverviewState();
}

class _AnnouncementsOverviewState extends State<AnnouncementsOverview> {
  List<Announcement> _announcements = [];

  @override
  void initState() {
    super.initState();

    _reloadAnnouncements();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text(context.l10n.announcementsOverview_title)),
      body: Card(
        child: Padding(
          padding: const EdgeInsets.all(8),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Row(
                crossAxisAlignment: CrossAxisAlignment.end,
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  if (kIsDesktop)
                    IconButton(icon: const Icon(Icons.refresh), onPressed: () async => _reloadAnnouncements(), tooltip: context.l10n.reload),
                  IconButton.filled(
                    icon: const Icon(Icons.add),
                    onPressed: () => showCreateAnnouncementDialog(context: context, onAnnouncementCreated: _reloadAnnouncements),
                  ),
                ],
              ),
              Expanded(
                child: DataTable2(
                  empty: Text(context.l10n.announcementsOverview_noAnnouncementsFound),
                  columns: <DataColumn2>[
                    DataColumn2(label: Text(context.l10n.title)),
                    DataColumn2(label: Text(context.l10n.createdAt)),
                    DataColumn2(label: Text(context.l10n.expiresAt)),
                    DataColumn2(label: Text(context.l10n.announcementsOverview_severity)),
                  ],
                  rows:
                      _announcements
                          .map(
                            (announcement) => DataRow2(
                              onTap: () => context.go('/announcements/${announcement.id}'),
                              cells: [
                                DataCell(Text(_getAnnouncementTitle(announcement, 'en'))),
                                DataCell(
                                  Tooltip(
                                    message:
                                        '${DateFormat.yMd(Localizations.localeOf(context).languageCode).format(announcement.createdAt)} ${DateFormat.Hms().format(announcement.createdAt)}',
                                    child: Text(DateFormat.yMd(Localizations.localeOf(context).languageCode).format(announcement.createdAt)),
                                  ),
                                ),
                                DataCell(
                                  Tooltip(
                                    message:
                                        announcement.expiresAt != null
                                            ? '${DateFormat.yMd(Localizations.localeOf(context).languageCode).format(announcement.expiresAt!)} ${DateFormat.Hms().format(announcement.expiresAt!)}'
                                            : '',
                                    child: Text(
                                      announcement.expiresAt != null
                                          ? DateFormat.yMd(Localizations.localeOf(context).languageCode).format(announcement.expiresAt!)
                                          : '',
                                    ),
                                  ),
                                ),
                                DataCell(Text(announcement.severity)),
                              ],
                            ),
                          )
                          .toList(),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  String _getAnnouncementTitle(Announcement announcement, String language) {
    return announcement.texts.firstWhere((t) => t.language == language).title;
  }

  Future<void> _reloadAnnouncements() async {
    final response = await GetIt.I.get<AdminApiClient>().announcements.getAnnouncements();
    final announcements = response.data..sort((a, b) => a.createdAt.compareTo(b.createdAt));

    setState(() {
      _announcements = announcements;
    });
  }
}
