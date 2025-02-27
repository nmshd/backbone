import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

import '/core/core.dart';
import '../widgets/language_picker.dart';

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
  final List<AnnouncementSeverity> _severityOptions = AnnouncementSeverity.values;

  final List<_AnnouncementTextFormWidget> _announcementTextWidgets = [];

  @override
  void initState() {
    super.initState();

    _announcementTextWidgets.add(_AnnouncementTextFormWidget(defaultLanguage: 'en', formKey: _formKey, onRemove: _remove));
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
                  items:
                      _severityOptions.map((severity) {
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
                Row(
                  children: [
                    Expanded(child: Text(context.l10n.createAnnouncement_defaultLanguage)),
                    IconButton.filled(
                      icon: const Icon(Icons.add),
                      onPressed: () {
                        setState(() {
                          _announcementTextWidgets.add(_AnnouncementTextFormWidget(formKey: _formKey, onRemove: _remove));
                        });
                      },
                    ),
                  ],
                ),
                Gaps.h16,
                Column(spacing: 16, children: _announcementTextWidgets),
              ],
            ),
          ),
        ),
      ),
      actions: [
        SizedBox(height: 40, child: OutlinedButton(onPressed: () => Navigator.of(context).pop(), child: Text(context.l10n.cancel))),
        SizedBox(
          height: 40,
          child: FilledButton(
            onPressed: () async {
              if (_formKey.currentState!.validate()) {
                final announcementTexts = <AnnouncementText>[];

                for (final announcementTextWidget in _announcementTextWidgets) {
                  final title = announcementTextWidget.titleController.text;
                  final body = announcementTextWidget.bodyController.text;
                  final language =
                      announcementTextWidget.languageController.text == ''
                          ? announcementTextWidget.defaultLanguage
                          : announcementTextWidget.languageController.text;

                  announcementTexts.add(AnnouncementText(title: title, body: body, language: language!));
                }

                final response = await GetIt.I.get<AdminApiClient>().announcements.createAnnouncement(
                  expiresAt: _selectedExpirationDate?.toIso8601String(),
                  severity: _selectedSeverity!,
                  announcementTexts: announcementTexts,
                  recipients: [],
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

  void _remove(_AnnouncementTextFormWidget announcementTextWidget) {
    final index = _announcementTextWidgets.indexWhere((element) => element == announcementTextWidget);

    setState(() {
      _announcementTextWidgets.removeAt(index);
    });
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

class _AnnouncementTextFormWidget extends StatefulWidget {
  final GlobalKey<FormState> formKey;
  final void Function(_AnnouncementTextFormWidget index) onRemove;

  _AnnouncementTextFormWidget({required this.formKey, required this.onRemove, this.defaultLanguage});

  @override
  State<_AnnouncementTextFormWidget> createState() => _AnnouncementTextFormWidgetState();

  final String? defaultLanguage;
  final TextEditingController titleController = TextEditingController();
  final TextEditingController bodyController = TextEditingController();
  final TextEditingController languageController = TextEditingController();
}

class _AnnouncementTextFormWidgetState extends State<_AnnouncementTextFormWidget> {
  String? selectedLanguage;
  @override
  void dispose() {
    widget.titleController.dispose();
    widget.bodyController.dispose();
    widget.languageController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: EdgeInsets.zero,
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Column(
          spacing: 16,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (widget.defaultLanguage == null) ...[
              Row(
                mainAxisAlignment: MainAxisAlignment.end,
                children: [TextButton(onPressed: () => widget.onRemove(widget), child: Text(context.l10n.remove))],
              ),
              LanguagePicker(
                width: MediaQuery.of(context).size.width,
                labelText: '${context.l10n.announcementsLanguage}*',
                onLanguageChanged: (selectedLanguage) {
                  setState(() {
                    widget.languageController.text = selectedLanguage;
                  });
                },
                validator: (value) => validateRequiredField(context, widget.languageController.text),
              ),
            ],
            TextFormField(
              controller: widget.titleController,
              validator: (value) => validateRequiredField(context, value),
              decoration: InputDecoration(labelText: '${context.l10n.title}*', border: const OutlineInputBorder()),
            ),
            TextFormField(
              controller: widget.bodyController,
              validator: (value) => validateRequiredField(context, value),
              decoration: InputDecoration(labelText: '${context.l10n.body}*', border: const OutlineInputBorder()),
            ),
          ],
        ),
      ),
    );
  }
}

String? validateRequiredField(BuildContext context, String? value) {
  if (value == null || value.isEmpty) {
    return context.l10n.createAnnouncementDialog_requiredField;
  }
  return null;
}
