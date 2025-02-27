import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';

class LanguagePicker extends StatelessWidget {
  final FormFieldValidator<String>? validator;
  final String labelText;
  final ValueChanged<String> onLanguageChanged;
  final double? width;

  const LanguagePicker({required this.labelText, required this.onLanguageChanged, super.key, this.validator, this.width});

  @override
  Widget build(BuildContext context) {
    final languageOptions =
        AnnouncementLanguages.languages.toList()..sort((a, b) {
          if (a.isoCode == 'de') return -1;
          if (b.isoCode == 'de') return 1;
          return a.name.compareTo(b.name);
        });
    return FormField<String>(
      validator: validator,
      builder: (fieldState) {
        return Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            DropdownMenu(
              menuHeight: MediaQuery.of(context).size.height * 0.3,
              expandedInsets: EdgeInsets.zero,
              errorText: fieldState.errorText,
              inputDecorationTheme:
                  fieldState.hasError
                      ? InputDecorationTheme(
                        border: OutlineInputBorder(borderSide: BorderSide(color: Theme.of(context).colorScheme.error)),
                        enabledBorder: OutlineInputBorder(borderSide: BorderSide(color: Theme.of(context).colorScheme.error)),
                      )
                      : const InputDecorationTheme(border: OutlineInputBorder()),
              label: Text(labelText, style: fieldState.hasError ? TextStyle(color: Theme.of(context).colorScheme.error) : null),
              requestFocusOnTap: true,
              enableFilter: true,
              dropdownMenuEntries:
                  languageOptions.map((language) {
                    return DropdownMenuEntry(value: language.isoCode, label: language.name);
                  }).toList(),
              onSelected: (String? selectedLanguageIsoCode) {
                if (selectedLanguageIsoCode != null) {
                  onLanguageChanged(selectedLanguageIsoCode);
                }
              },
            ),
          ],
        );
      },
    );
  }
}
