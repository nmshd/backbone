import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:enmeshed_ui_kit/enmeshed_ui_kit.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

import '/core/core.dart';

Future<void> showCreateAnnouncementDialog({required BuildContext context, required VoidCallback onAnnouncementCreated}) async {
  await showDialog<void>(
    context: context,
    builder: (BuildContext context) => _CreateAnnouncementDialog(onAnnouncementCreated: onAnnouncementCreated),
  );
}

class _CreateAnnouncementDialog extends StatefulWidget {
  final VoidCallback onAnnouncementCreated;

  const _CreateAnnouncementDialog({required this.onAnnouncementCreated});

  @override
  State<_CreateAnnouncementDialog> createState() => _CreateAnnouncementDialogState();
}

class _CreateAnnouncementDialogState extends State<_CreateAnnouncementDialog> {
  final _formKey = GlobalKey<FormState>();

  AnnouncementSeverity? _selectedSeverity;
  DateTime? _selectedExpirationDate;
  bool _sendAPushNotification = false;
  String _iqlQuery = '';
  String _englishTitle = '';
  String _englishBody = '';
  String _germanTitle = '';
  String _germanBody = '';
  final List<AnnouncementSeverity> _severityOptions = AnnouncementSeverity.values;

  @override
  void initState() {
    super.initState();
  }

  @override
  void dispose() {
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      scrollable: true,
      backgroundColor: Theme.of(context).colorScheme.surface,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
      title: Text(context.l10n.createAnnouncementDialog_title, textAlign: TextAlign.center),
      contentPadding: const EdgeInsets.only(left: 24, right: 24, top: 20, bottom: 32),
      content: SizedBox(
        width: 500,
        child: SingleChildScrollView(
          child: Form(
            key: _formKey,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisSize: MainAxisSize.min,
              children: [
                Text(context.l10n.createAnnouncementDialog_explanation),
                Gaps.h32,
                Text('*${context.l10n.required}'),
                Gaps.h16,
                DropdownButtonFormField(
                  validator: (value) => validateRequiredField(context, value?.name),
                  decoration: InputDecoration(labelText: '${context.l10n.createAnnouncementDialog_impact}*', border: const OutlineInputBorder()),
                  items: _severityOptions.map((severity) {
                    return DropdownMenuItem<AnnouncementSeverity>(value: severity, child: Text(severity.name));
                  }).toList(),
                  onChanged: (newValue) => setState(() => _selectedSeverity = newValue),
                ),
                Gaps.h16,
                TextFormField(
                  controller: TextEditingController(text: _selectedExpirationDate != null ? DateFormat.yMd().format(_selectedExpirationDate!) : ''),
                  decoration: InputDecoration(
                    labelText: context.l10n.expiresAt,
                    border: const OutlineInputBorder(),
                    suffixIcon: IconButton(
                      icon: const Icon(Icons.calendar_today),
                      onPressed: () async {
                        final selectedDate = await showDatePicker(
                          context: context,
                          initialDate: DateTime.now(),
                          firstDate: DateTime.now(),
                          lastDate: DateTime.now().add(const Duration(days: 365)),
                        );

                        if (selectedDate != null) {
                          setState(() => _selectedExpirationDate = selectedDate);
                        }
                      },
                    ),
                  ),
                  onTap: () async {
                    final selectedDate = await showDatePicker(
                      context: context,
                      initialDate: DateTime.now(),
                      firstDate: DateTime.now(),
                      lastDate: DateTime.now().add(const Duration(days: 365)),
                    );

                    if (selectedDate != null) {
                      setState(() => _selectedExpirationDate = selectedDate);
                    }
                  },
                ),
                Gaps.h16,
                CheckboxListTile(
                  title: Text(context.l10n.createAnnouncementDialog_sendAPushNotification),
                  value: _sendAPushNotification,
                  onChanged: (value) => setState(() => _sendAPushNotification = value ?? false),
                  contentPadding: EdgeInsets.zero,
                  controlAffinity: ListTileControlAffinity.leading,
                ),
                Gaps.h16,
                TextFormField(
                  initialValue: _iqlQuery,
                  decoration: InputDecoration(labelText: context.l10n.createAnnouncementDialog_iqlQuery, border: const OutlineInputBorder()),
                  onChanged: (value) => _iqlQuery = value,
                ),
                Gaps.h32,
                TextFormField(
                  initialValue: _englishTitle,
                  decoration: InputDecoration(
                    labelText: '${context.l10n.createAnnouncementDialog_englishTitle}*',
                    border: const OutlineInputBorder(),
                  ),
                  validator: (value) => validateRequiredField(context, value),
                  onChanged: (value) => _englishTitle = value,
                ),
                Gaps.h16,
                TextFormField(
                  initialValue: _englishBody,
                  decoration: InputDecoration(labelText: '${context.l10n.createAnnouncementDialog_englishBody}*', border: const OutlineInputBorder()),
                  validator: (value) => validateRequiredField(context, value),
                  onChanged: (value) => _englishBody = value,
                ),
                Gaps.h32,
                TextFormField(
                  initialValue: _germanTitle,
                  decoration: InputDecoration(labelText: context.l10n.createAnnouncementDialog_germanTitle, border: const OutlineInputBorder()),
                  onChanged: (value) => _germanTitle = value,
                  validator: (value) {
                    if ((value == null || value.isEmpty) && (_germanBody.isNotEmpty)) {
                      return context.l10n.createAnnouncementDialog_titleAndBodyAreRequired;
                    }
                    return null;
                  },
                ),
                Gaps.h16,
                TextFormField(
                  initialValue: _germanBody,
                  decoration: InputDecoration(labelText: context.l10n.createAnnouncementDialog_germanBody, border: const OutlineInputBorder()),
                  onChanged: (value) => _germanBody = value,
                  validator: (value) {
                    if ((value == null || value.isEmpty) && (_germanTitle.isNotEmpty)) {
                      return context.l10n.createAnnouncementDialog_titleAndBodyAreRequired;
                    }
                    return null;
                  },
                ),
              ],
            ),
          ),
        ),
      ),
      actions: [
        SizedBox(
          height: 40,
          child: OutlinedButton(onPressed: () => Navigator.of(context).pop(), child: Text(context.l10n.cancel)),
        ),
        SizedBox(
          height: 40,
          child: FilledButton(
            onPressed: () async {
              if (_formKey.currentState!.validate()) {
                final announcementTexts = <AnnouncementText>[AnnouncementText(language: 'en', title: _englishTitle, body: _englishBody)];
                if (_germanTitle.isNotEmpty && _germanBody.isNotEmpty) {
                  announcementTexts.add(AnnouncementText(language: 'de', title: _germanTitle, body: _germanBody));
                }

                final response = await GetIt.I.get<AdminApiClient>().announcements.createAnnouncement(
                  expiresAt: _selectedExpirationDate?.toIso8601String(),
                  severity: _selectedSeverity!,
                  announcementTexts: announcementTexts,
                  recipients: [],
                  iqlQuery: _iqlQuery,
                  isSilent: !_sendAPushNotification,
                );

                if (!context.mounted) return;
                context.pop();

                if (response.hasError) {
                  _showErrorSnackbar();
                  return;
                }
                widget.onAnnouncementCreated();
                _showSuccessSnackbar();
              }
            },
            child: Text(context.l10n.create),
          ),
        ),
      ],
    );
  }

  void _showSuccessSnackbar() {
    final snackBar = SnackBar(
      content: Text(context.l10n.createAnnouncement_announcementSuccess, style: const TextStyle(color: Colors.white)),
      backgroundColor: Colors.green,
      duration: const Duration(seconds: 3),
      showCloseIcon: true,
    );
    ScaffoldMessenger.of(context).showSnackBar(snackBar);
  }

  void _showErrorSnackbar() {
    final snackBar = SnackBar(
      content: Text(context.l10n.createAnnouncement_announcementError, style: const TextStyle(color: Colors.white)),
      backgroundColor: Colors.red,
      duration: const Duration(seconds: 3),
      showCloseIcon: true,
    );
    ScaffoldMessenger.of(context).showSnackBar(snackBar);
  }
}

String? validateRequiredField(BuildContext context, String? value) {
  if (value == null || value.isEmpty) {
    return context.l10n.createAnnouncementDialog_requiredField;
  }
  return null;
}
