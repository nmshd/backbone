import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';

class AnnouncementDetails extends StatefulWidget {
  final String announcementId;

  const AnnouncementDetails({
    required this.announcementId,
    super.key,
  });

  @override
  State<AnnouncementDetails> createState() => _AnnouncementDetailsState();
}

class _AnnouncementDetailsState extends State<AnnouncementDetails> {
  AnnouncementOverview? _announcmentDetails;

  @override
  void initState() {
    super.initState();

    _loadAnnouncement();
  }

  @override
  void dispose() {
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    if (_announcmentDetails == null) return const Center(child: CircularProgressIndicator());

    return SingleChildScrollView(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          AnnouncementsTextTable(announcementTexts: _announcmentDetails!.texts),
        ],
      ),
    );
  }

  Future<void> _loadAnnouncement() async {
    final announcementDetailsResponse = await GetIt.I.get<AdminApiClient>().announcements.getAnnouncement(widget.announcementId);

    if (!mounted) return;

    setState(() {
      _announcmentDetails = announcementDetailsResponse.data;
    });
  }
}

class AnnouncementsTextTable extends StatelessWidget {
  final List<AnnouncementText> announcementTexts;

  const AnnouncementsTextTable({
    required this.announcementTexts,
    super.key,
  });

  @override
  Widget build(BuildContext context) {
    return DataTable(
      columns: const <DataColumn>[
        DataColumn(label: Text('Language')),
        DataColumn(label: Text('Title')),
        DataColumn(label: Text('Text')),
      ],
      rows: announcementTexts
          .map(
            (announcementText) => DataRow(
              cells: [
                DataCell(Text(announcementText.language)),
                DataCell(Text(announcementText.title)),
                DataCell(Text(announcementText.body)),
              ],
            ),
          )
          .toList(),
    );
  }
}
