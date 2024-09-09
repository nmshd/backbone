import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';

import '/core/core.dart';
import 'modals/create_announcement_dialog.dart';

class AnnouncementsOverview extends StatefulWidget {
  const AnnouncementsOverview({super.key});

  @override
  State<AnnouncementsOverview> createState() => _AnnouncementsOverviewState();
}

class _AnnouncementsOverviewState extends State<AnnouncementsOverview> {
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
                    IconButton(
                      icon: const Icon(Icons.refresh),
                      onPressed: () async {},
                      tooltip: context.l10n.reload,
                    ),
                  IconButton.filled(
                    icon: const Icon(Icons.add),
                    onPressed: () => showCreateAnnouncementDialog(context: context, onAnnouncementCreated: () {}),
                  ),
                ],
              ),
              Expanded(
                child: DataTable2(
                  columns: <DataColumn2>[
                    DataColumn2(label: Text(context.l10n.id)),
                    DataColumn2(label: Text(context.l10n.createdAt)),
                    DataColumn2(label: Text(context.l10n.expiresAt)),
                    DataColumn2(label: Text(context.l10n.announcementsOverview_severity)),
                    const DataColumn2(label: Text('')),
                  ],
                  rows: const [],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Future<void> _reloadAnnouncements() async {
    setState(() {});
  }
}
