import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:admin_ui/core/constants.dart';
import 'package:flutter/material.dart';

class LanguagePicker extends StatelessWidget {
  final FormFieldValidator<String>? validator;
  final String labelText;
  final ValueChanged<String> onLanguageChanged;
  final double? width;

  const LanguagePicker({
    required this.labelText,
    required this.onLanguageChanged,
    super.key,
    this.validator,
    this.width,
  });

  @override
  Widget build(BuildContext context) {
    final languageOptions = Languages.languages.toList()
      ..sort((a, b) {
        if (a.isoCode == 'de') return -1;
        if (b.isoCode == 'de') return 1;
        return a.name.compareTo(b.name);
      });
    return FormField<String>(
      validator: validator,
      builder: (fieldState) {
        return Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            DropdownMenu(
              menuHeight: 300,
              inputDecorationTheme: const InputDecorationTheme(
                border: OutlineInputBorder(),
              ),
              label: Text(labelText),
              requestFocusOnTap: true,
              enableFilter: true,
              dropdownMenuEntries: languageOptions.map((language) {
                return DropdownMenuEntry(
                  value: language.isoCode,
                  label: language.name,
                );
              }).toList(),
              onSelected: (String? selectedLanguageIsoCode) {
                if (selectedLanguageIsoCode != null) {
                  onLanguageChanged(selectedLanguageIsoCode);
                }
              },
            ),
            if (fieldState.hasError) ...[
              Gaps.h4,
              Text(fieldState.errorText!, style: const TextStyle(color: Colors.redAccent)),
            ],
          ],
        );
      },
    );
  }
}
