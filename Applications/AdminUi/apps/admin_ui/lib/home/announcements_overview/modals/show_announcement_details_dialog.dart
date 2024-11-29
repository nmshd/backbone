import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';

import '/core/core.dart';

Future<void> showAnnouncementDetailsDialog({
  required BuildContext context,
  required List<AnnouncementText> announcementTexts,
}) async {
  await showDialog<void>(
    context: context,
    builder: (BuildContext context) => _AnnouncementTextDialog(announcementTexts: announcementTexts),
  );
}

class _AnnouncementTextDialog extends StatelessWidget {
  final List<AnnouncementText> announcementTexts;

  const _AnnouncementTextDialog({required this.announcementTexts});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 500,
      child: AlertDialog(
        title: Text(context.l10n.announcementsDialog_details),
        content: SingleChildScrollView(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              for (final announcementText in announcementTexts) ...[
                Text('${context.l10n.title}: ${announcementText.title}', style: const TextStyle(fontWeight: FontWeight.bold)),
                const SizedBox(height: 4),
                Text('${context.l10n.body}: ${announcementText.body}'),
                const SizedBox(height: 16),
              ],
            ],
          ),
        ),
        actions: [
          SizedBox(
            height: 40,
            child: OutlinedButton(
              child: Text(context.l10n.close),
              onPressed: () => Navigator.of(context).pop(),
            ),
          ),
        ],
      ),
    );
  }
}
