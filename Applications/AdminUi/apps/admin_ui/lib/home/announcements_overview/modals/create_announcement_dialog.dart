import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

import '/core/core.dart';
import '../widgets/language_picker.dart';

Future<void> showCreateAnnouncementDialog({
  required BuildContext context,
  required VoidCallback onAnnouncementCreated,
}) async {
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

  String? _selectedSeverity;
  DateTime? _selectedExpirationDate;
  final List<SeverityType> _severityOptions = SeverityType.values;

  final _announcementTextWidgets = <_AnnouncementTextFormWidget>[];

  @override
  void initState() {
    super.initState();

    _announcementTextWidgets.add(
      _AnnouncementTextFormWidget(
        language: 'en',
        formKey: _formKey,
        onRemove: _remove,
      ),
    );
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
                  validator: (value) => validateRequiredField(context, value),
                  decoration: InputDecoration(
                    labelText: '${context.l10n.createAnnouncementDialog_impact}*',
                    border: const OutlineInputBorder(),
                  ),
                  items: _severityOptions.map((severity) {
                    return DropdownMenuItem<String>(value: severity.name, child: Text(severity.name));
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
                    const Expanded(child: Text('Languages (default is English)')),
                    IconButton.filled(
                      icon: const Icon(Icons.add),
                      onPressed: () {
                        setState(() {
                          _announcementTextWidgets.add(
                            _AnnouncementTextFormWidget(
                              formKey: _formKey,
                              onRemove: _remove,
                            ),
                          );
                        });
                      },
                    ),
                  ],
                ),
                Gaps.h8,
                Column(
                  children: _announcementTextWidgets,
                ),
              ],
            ),
          ),
        ),
      ),
      actions: [
        SizedBox(
          height: 40,
          child: OutlinedButton(
            onPressed: () => Navigator.of(context).pop(),
            child: Text(context.l10n.cancel),
          ),
        ),
        SizedBox(
          height: 40,
          child: FilledButton(
            onPressed: () async {
              if (_formKey.currentState!.validate()) {
                final announcementTexts = <AnnouncementText>[];

                for (final announcementTextWidget in _announcementTextWidgets) {
                  final title = announcementTextWidget._titleController.text;
                  final body = announcementTextWidget._bodyController.text;
                  final language = announcementTextWidget.language;

                  announcementTexts.add(
                    AnnouncementText(
                      title: title,
                      body: body,
                      language: language!,
                    ),
                  );
                }

                await GetIt.I.get<AdminApiClient>().announcements.createAnnouncement(
                  expiresAt: _selectedExpirationDate?.toIso8601String(),
                  severity: _selectedSeverity!,
                  announcementTexts: announcementTexts,
                  recipients: [],
                );

                widget.onAnnouncementCreated();
                _showSnackbar();
                if (mounted) {
                  context.pop();
                }
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

  void _showSnackbar() {
    final snackBar = SnackBar(
      content: Text(
        context.l10n.createAnnouncement_announcementSuccess,
        style: const TextStyle(color: Colors.white),
      ),
      backgroundColor: Colors.green,
      duration: const Duration(seconds: 3),
      showCloseIcon: true,
    );
    ScaffoldMessenger.of(context).showSnackBar(snackBar);
  }
}

class _AnnouncementTextFormWidget extends StatefulWidget {
  final GlobalKey<FormState> formKey;
  final void Function(_AnnouncementTextFormWidget index) onRemove;
  _AnnouncementTextFormWidget({
    required this.formKey,
    required this.onRemove,
    this.language,
  });

  @override
  State<_AnnouncementTextFormWidget> createState() => _AnnouncementTextFormWidgetState();

  final String? language;
  final TextEditingController _titleController = TextEditingController();
  final TextEditingController _bodyController = TextEditingController();
}

class _AnnouncementTextFormWidgetState extends State<_AnnouncementTextFormWidget> {
  @override
  Widget build(BuildContext context) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.symmetric(vertical: 8),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (widget.language == null || widget.language!.isEmpty) ...[
              Row(
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  TextButton(onPressed: () => widget.onRemove(widget), child: const Text('Remove')),
                ],
              ),
              LanguagePicker(
                selectedLanguages: const [],
                onSelectedLanguagesChanged: (languages) {},
                validator: (value) => validateRequiredField(context, value),
              ),
              Gaps.h8,
            ],
            TextFormField(
              controller: widget._titleController,
              validator: (value) => validateRequiredField(context, value),
              decoration: InputDecoration(
                labelText: '${context.l10n.title}*',
                border: const OutlineInputBorder(),
              ),
            ),
            Gaps.h8,
            TextFormField(
              controller: widget._bodyController,
              validator: (value) => validateRequiredField(context, value),
              decoration: InputDecoration(
                labelText: '${context.l10n.body}*',
                border: const OutlineInputBorder(),
              ),
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
