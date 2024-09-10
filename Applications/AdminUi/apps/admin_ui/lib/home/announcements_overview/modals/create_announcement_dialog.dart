import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

import '/core/core.dart';
import '../widgets/language_multi_select.dart';

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
  bool _saving = false;
  bool _saveSucceeded = false;

  DateTime? _expiresAt;
  String? _selectedImpact;

  List<String> _selectedLanguages = [];
  final List<SeverityType> _severityOptions = SeverityType.values;

  final _englishTitleController = TextEditingController();
  final _englishBodyController = TextEditingController();
  final Map<OptionalLanguageType, TextEditingController> _titleControllers = {};
  final Map<OptionalLanguageType, TextEditingController> _bodyControllers = {};

  @override
  void initState() {
    super.initState();

    for (final language in OptionalLanguageType.values) {
      _titleControllers[language] = TextEditingController();
      _bodyControllers[language] = TextEditingController();
    }
  }

  @override
  void dispose() {
    _englishTitleController.dispose();
    _englishBodyController.dispose();

    for (final controller in _titleControllers.values) {
      controller.dispose();
    }

    for (final controller in _bodyControllers.values) {
      controller.dispose();
    }

    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return PopScope(
      canPop: !_saving,
      child: AlertDialog(
        backgroundColor: Theme.of(context).colorScheme.surface,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
        title: Text(context.l10n.createAnnouncementDialog_title, textAlign: TextAlign.center),
        contentPadding: const EdgeInsets.only(left: 24, right: 24, top: 20, bottom: 32),
        content: SizedBox(
          width: 500,
          child: SingleChildScrollView(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(context.l10n.createAnnouncementDialog_explanation),
                Gaps.h32,
                Text('*${context.l10n.required}'),
                Gaps.h16,
                _AnnouncementField(label: '${context.l10n.title} (${context.l10n.createAnnouncement_english})*', controller: _englishTitleController),
                _AnnouncementField(label: '${context.l10n.body} (${context.l10n.createAnnouncement_english})*', controller: _englishBodyController),
                LanguageMultiSelect(
                  selectedLanguages: _selectedLanguages,
                  onSelectedLanguagesChanged: (selectedLanguages) => setState(() => _selectedLanguages = selectedLanguages),
                ),
                Gaps.h16,
                if (_selectedLanguages.contains(OptionalLanguageType.german.name)) ...[
                  _AnnouncementField(
                    label: '${context.l10n.title} (${context.l10n.createAnnouncement_german})',
                    controller: _titleControllers[OptionalLanguageType.german]!,
                  ),
                  _AnnouncementField(
                    label: '${context.l10n.body} (${context.l10n.createAnnouncement_german})',
                    controller: _bodyControllers[OptionalLanguageType.german]!,
                  ),
                ],
                if (_selectedLanguages.contains(OptionalLanguageType.portuguese.name)) ...[
                  _AnnouncementField(
                    label: '${context.l10n.title} (${context.l10n.createAnnouncement_portuguese})',
                    controller: _titleControllers[OptionalLanguageType.portuguese]!,
                  ),
                  _AnnouncementField(
                    label: '${context.l10n.body} (${context.l10n.createAnnouncement_portuguese})',
                    controller: _bodyControllers[OptionalLanguageType.portuguese]!,
                  ),
                ],
                if (_selectedLanguages.contains(OptionalLanguageType.italian.name)) ...[
                  _AnnouncementField(
                    label: '${context.l10n.title} (${context.l10n.createAnnouncement_italian})',
                    controller: _titleControllers[OptionalLanguageType.italian]!,
                  ),
                  _AnnouncementField(
                    label: '${context.l10n.body} (${context.l10n.createAnnouncement_italian})',
                    controller: _bodyControllers[OptionalLanguageType.italian]!,
                  ),
                ],
                Gaps.h8,
                TextFormField(
                  readOnly: true,
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
                          lastDate: DateTime(2100),
                        );

                        if (selectedDate != null) setState(() => _expiresAt = selectedDate);
                      },
                    ),
                  ),
                  controller: TextEditingController(text: _expiresAt != null ? DateFormat.yMd().format(_expiresAt!) : ''),
                ),
                Gaps.h16,
                DropdownButtonFormField<String>(
                  value: _selectedImpact,
                  decoration: InputDecoration(labelText: context.l10n.createAnnouncementDialog_impact, border: const OutlineInputBorder()),
                  items: _severityOptions.map((value) {
                    return DropdownMenuItem<String>(value: value.name, child: Text(value.name));
                  }).toList(),
                  onChanged: (newValue) => setState(() => _selectedImpact = newValue),
                ),
              ],
            ),
          ),
        ),
        actions: [
          SizedBox(height: 40, child: OutlinedButton(onPressed: _saving ? null : () => context.pop(), child: Text(context.l10n.cancel))),
          if (!_saveSucceeded)
            SizedBox(
              height: 40,
              child: FilledButton(onPressed: !_saveSucceeded && !_saving ? _createAnnouncement : null, child: Text(context.l10n.create)),
            ),
        ],
      ),
    );
  }

  Future<void> _createAnnouncement() async {
    setState(() => _saving = true);

    setState(() => _saving = false);
    widget.onAnnouncementCreated();
    setState(() => _saveSucceeded = true);
  }
}

class _AnnouncementField extends StatelessWidget {
  final TextEditingController controller;
  final String label;

  const _AnnouncementField({
    required this.controller,
    required this.label,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 16),
      child: TextField(
        controller: controller,
        decoration: InputDecoration(
          labelText: label,
          border: const OutlineInputBorder(),
          contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
        ),
      ),
    );
  }
}
