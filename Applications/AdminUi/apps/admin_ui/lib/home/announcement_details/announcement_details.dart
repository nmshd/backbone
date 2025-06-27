import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:data_table_2/data_table_2.dart';
import 'package:enmeshed_ui_kit/enmeshed_ui_kit.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:intl/intl.dart';

import '/core/core.dart';

class AnnouncementDetails extends StatefulWidget {
  final String announcementId;

  const AnnouncementDetails({required this.announcementId, super.key});

  @override
  State<AnnouncementDetails> createState() => _AnnouncementDetailsState();
}

class _AnnouncementDetailsState extends State<AnnouncementDetails> {
  Announcement? _announcementDetails;

  late final ScrollController _scrollController;

  @override
  void initState() {
    super.initState();

    _scrollController = ScrollController();

    _loadAnnouncement();
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    if (_announcementDetails == null) return const Center(child: CircularProgressIndicator());

    final announcementDetails = _announcementDetails!;

    return Scrollbar(
      controller: _scrollController,
      child: SingleChildScrollView(
        controller: _scrollController,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            if (kIsDesktop)
              Row(
                children: [
                  const BackButton(),
                  IconButton(icon: const Icon(Icons.refresh), onPressed: _loadAnnouncement, tooltip: context.l10n.reload),
                ],
              ),
            Card(
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    Text(context.l10n.announcementDetails, style: Theme.of(context).textTheme.headlineLarge),
                    const SizedBox(height: 32),
                    Wrap(
                      crossAxisAlignment: WrapCrossAlignment.center,
                      spacing: 8,
                      runSpacing: 8,
                      children: [
                        EntityDetails(title: context.l10n.announcementsOverview_severity, value: announcementDetails.severity),
                        EntityDetails(
                          title: context.l10n.createdAt,
                          value: DateFormat.yMd(Localizations.localeOf(context).languageCode).format(announcementDetails.createdAt),
                        ),
                        if (announcementDetails.expiresAt != null)
                          EntityDetails(
                            title: context.l10n.expiresAt,
                            value: DateFormat.yMd(Localizations.localeOf(context).languageCode).format(announcementDetails.expiresAt!),
                          ),
                        if (announcementDetails.iqlQuery != null)
                          EntityDetails(title: context.l10n.announcementDetails_iqlQuery, value: announcementDetails.iqlQuery!),
                        EntityDetails(
                          title: context.l10n.announcementDetails_sendAPushNotification,
                          value: announcementDetails.isSilent ? context.l10n.no : context.l10n.yes,
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ),
            _AnnouncementTextsCard(announcementTexts: announcementDetails.texts),
            _AnnouncementActionsCard(announcementActions: announcementDetails.actions),
          ],
        ),
      ),
    );
  }

  Future<void> _loadAnnouncement() async {
    final announcementDetailsResponse = await GetIt.I.get<AdminApiClient>().announcements.getAnnouncement(widget.announcementId);

    if (!mounted) return;

    setState(() {
      _announcementDetails = announcementDetailsResponse.data;
    });
  }
}

class _AnnouncementTextsCard extends StatelessWidget {
  final List<AnnouncementText> announcementTexts;

  const _AnnouncementTextsCard({required this.announcementTexts});

  @override
  Widget build(BuildContext context) {
    return Card(
      child: SizedBox(
        width: double.infinity,
        height: 200,
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

class _AnnouncementActionsCard extends StatelessWidget {
  final List<AnnouncementAction> announcementActions;

  const _AnnouncementActionsCard({required this.announcementActions});

  @override
  Widget build(BuildContext context) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(context.l10n.announcementDetails_actions, style: Theme.of(context).textTheme.titleMedium),
            Gaps.h12,
            Wrap(
              crossAxisAlignment: WrapCrossAlignment.center,
              spacing: 8,
              runSpacing: 8,
              children: announcementActions
                  .map(
                    (action) => SizedBox(
                      width: 500,
                      child: Card(
                        elevation: 3,
                        child: Padding(
                          padding: const EdgeInsets.all(16),
                          child: Table(
                            columnWidths: const <int, TableColumnWidth>{
                              0: IntrinsicColumnWidth(),
                              1: FlexColumnWidth(),
                            },
                            children: [
                              TableRow(
                                children: [
                                  Padding(
                                    padding: const EdgeInsets.only(top: 12, bottom: 12),
                                    child: Text(
                                      context.l10n.announcementDetails_actions_englishName,
                                      style: const TextStyle(fontWeight: FontWeight.bold),
                                    ),
                                  ),
                                  Padding(
                                    padding: const EdgeInsets.only(left: 18, top: 12, bottom: 12),
                                    child: Text(action.displayName['en'] ?? ''),
                                  ),
                                ],
                              ),
                              TableRow(
                                children: [
                                  Padding(
                                    padding: const EdgeInsets.only(top: 12, bottom: 12),
                                    child: Text(
                                      context.l10n.announcementDetails_actions_germanName,
                                      style: const TextStyle(fontWeight: FontWeight.bold),
                                    ),
                                  ),
                                  Padding(
                                    padding: const EdgeInsets.only(left: 18, top: 12, bottom: 12),
                                    child: Text(action.displayName['de'] ?? ''),
                                  ),
                                ],
                              ),
                              TableRow(
                                children: [
                                  Padding(
                                    padding: const EdgeInsets.only(top: 12, bottom: 12),
                                    child: Text(context.l10n.announcementDetails_actions_link, style: const TextStyle(fontWeight: FontWeight.bold)),
                                  ),
                                  Padding(
                                    padding: const EdgeInsets.only(left: 18, top: 12, bottom: 12),
                                    child: Text(action.link),
                                  ),
                                ],
                              ),
                            ],
                          ),
                        ),
                      ),
                    ),
                  )
                  .toList(),
            ),
          ],
        ),
      ),
    );
  }
}
