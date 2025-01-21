import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
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
  late String _errorMessage;

  bool _saving = false;
  final bool _saveSucceeded = false;
  bool _showRequiredFieldsError = false;

  DateTime? _expiresAt;
  String? _selectedImpact;

  List<String> _selectedLanguages = [];
  final List<SeverityType> _severityOptions = SeverityType.values;

  final _englishTextController = _LanguageTextController(
    titleController: TextEditingController(),
    bodyController: TextEditingController(),
  );
  late List<_LanguageTextController> _languageTextControllers;

  @override
  void initState() {
    super.initState();

    _errorMessage = '';

    _languageTextControllers = OptionalLanguageType.values.map((language) {
      return _LanguageTextController(
        titleController: TextEditingController(),
        bodyController: TextEditingController(),
      );
    }).toList();
  }

  @override
  void dispose() {
    _englishTextController.titleController.dispose();
    _englishTextController.bodyController.dispose();

    for (final controller in _languageTextControllers) {
      controller.titleController.dispose();
      controller.bodyController.dispose();
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
                if (_showRequiredFieldsError) ...[
                  Text(
                    context.l10n.createAnnouncement_pleaseFillAllRequiredFields,
                    style: TextStyle(color: Theme.of(context).colorScheme.error),
                  ),
                  Gaps.h16,
                ],
                if (_errorMessage != '') ...[
                  Text(
                    _errorMessage,
                    style: TextStyle(color: Theme.of(context).colorScheme.error),
                  ),
                  Gaps.h16,
                ],
                _AnnouncementField(
                  label: '${context.l10n.title} (${context.l10n.createAnnouncement_english})*',
                  controller: _englishTextController.titleController,
                ),
                _AnnouncementField(
                  label: '${context.l10n.body} (${context.l10n.createAnnouncement_english})*',
                  controller: _englishTextController.bodyController,
                ),
                LanguageMultiSelect(
                  selectedLanguages: _selectedLanguages,
                  onSelectedLanguagesChanged: (selectedLanguages) => setState(() => _selectedLanguages = selectedLanguages),
                ),
                Gaps.h16,
                ...OptionalLanguageType.values.where((language) => _selectedLanguages.contains(language.name)).map(
                      (language) => Column(
                        children: [
                          _AnnouncementField(
                            label: '${context.l10n.title} (${_getLanguageLabel(language)})',
                            controller: _languageTextControllers[language.index].titleController,
                          ),
                          _AnnouncementField(
                            label: '${context.l10n.body} (${_getLanguageLabel(language)})',
                            controller: _languageTextControllers[language.index].bodyController,
                          ),
                        ],
                      ),
                    ),
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
          SizedBox(
            height: 40,
            child: OutlinedButton(
              onPressed: _saving ? null : () => context.pop(),
              child: Text(context.l10n.cancel),
            ),
          ),
          if (!_saveSucceeded)
            SizedBox(
              height: 40,
              child: FilledButton(
                onPressed: !_saveSucceeded && !_saving ? _createAnnouncement : null,
                child: Text(context.l10n.create),
              ),
            ),
        ],
      ),
    );
  }

  Future<void> _createAnnouncement() async {
    if (_expiresAt == null ||
        _selectedImpact == null ||
        _englishTextController.titleController.text == '' ||
        _englishTextController.bodyController.text == '') {
      setState(() {
        _showRequiredFieldsError = true;
      });

      return;
    }

    setState(() {
      _showRequiredFieldsError = false;
      _saving = true;
    });

    final announcementTexts = <AnnouncementText>[
      AnnouncementText(
        language: 'en',
        title: _englishTextController.titleController.text,
        body: _englishTextController.bodyController.text,
      ),
    ];

    for (final language in OptionalLanguageType.values) {
      if (_selectedLanguages.contains(language.name)) {
        announcementTexts.add(
          AnnouncementText(
            language: switch (language) {
              OptionalLanguageType.german => 'de',
              OptionalLanguageType.portuguese => 'pt',
              OptionalLanguageType.italian => 'it',
            }, // TODO: find a better way
            title: _languageTextControllers[language.index].titleController.text,
            body: _languageTextControllers[language.index].bodyController.text,
          ),
        );
      }
    }

    final response = await GetIt.I.get<AdminApiClient>().announcements.createAnnouncement(
          expiresAt: _expiresAt!.toIso8601String(),
          severity: _selectedImpact!,
          announcementTexts: announcementTexts,
        );

    if (response.hasError) {
      _errorMessage = response.error.message;
      return;
    }

    _showSnackbar();
    widget.onAnnouncementCreated();
    //setState(() => _saveSucceeded = true);
    context.pop();
  }

  String _getLanguageLabel(OptionalLanguageType language) {
    switch (language) {
      case OptionalLanguageType.german:
        return context.l10n.createAnnouncement_german;
      case OptionalLanguageType.portuguese:
        return context.l10n.createAnnouncement_portuguese;
      case OptionalLanguageType.italian:
        return context.l10n.createAnnouncement_italian;
    }
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

class _LanguageTextController {
  final TextEditingController titleController;
  final TextEditingController bodyController;

  _LanguageTextController({required this.titleController, required this.bodyController});
}
