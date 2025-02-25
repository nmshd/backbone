import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:intl/intl.dart';

import '/core/core.dart';

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

    return Scrollbar(
      child: SingleChildScrollView(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (kIsDesktop)
              Row(
                children: [
                  const BackButton(),
                  IconButton(
                    icon: const Icon(Icons.refresh),
                    onPressed: _loadAnnouncement,
                    tooltip: context.l10n.reload,
                  ),
                ],
              ),
            Card(
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    Text(context.l10n.announcementsDetails, style: Theme.of(context).textTheme.headlineLarge),
                    const SizedBox(height: 32),
                    Wrap(
                      crossAxisAlignment: WrapCrossAlignment.center,
                      spacing: 8,
                      runSpacing: 8,
                      children: [
                        EntityDetails(title: context.l10n.announcementsOverview_severity, value: _announcmentDetails!.severity),
                        EntityDetails(
                          title: context.l10n.createdAt,
                          value: DateFormat.yMd(Localizations.localeOf(context).languageCode).format(_announcmentDetails!.createdAt),
                        ),
                        if (_announcmentDetails!.expiresAt != null)
                          EntityDetails(
                            title: context.l10n.expiresAt,
                            value: DateFormat.yMd(Localizations.localeOf(context).languageCode).format(_announcmentDetails!.expiresAt!),
                          ),
                      ],
                    ),
                  ],
                ),
              ),
            ),
            _AnnouncementsTextTable(announcementTexts: _announcmentDetails!.texts),
          ],
        ),
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

class _AnnouncementsTextTable extends StatelessWidget {
  final List<AnnouncementText> announcementTexts;

  const _AnnouncementsTextTable({
    required this.announcementTexts,
  });

  @override
  Widget build(BuildContext context) {
    return Card(
      child: SizedBox(
        width: double.infinity,
        height: 500,
        child: DataTable2(
          columns: <DataColumn>[
            DataColumn2(label: Text(context.l10n.announcementsLanguage)),
            DataColumn2(label: Text(context.l10n.title)),
            DataColumn2(label: Text(context.l10n.body)),
          ],
          rows: announcementTexts
              .map(
                (announcementText) => DataRow(
                  cells: [
                    DataCell(Text(AnnouncementLanguages.languages.firstWhere((language) => language.isoCode == announcementText.language).name)),
                    DataCell(Text(announcementText.title)),
                    DataCell(Text(announcementText.body)),
                  ],
                ),
              )
              .toList(),
        ),
      ),
    );
  }
}
