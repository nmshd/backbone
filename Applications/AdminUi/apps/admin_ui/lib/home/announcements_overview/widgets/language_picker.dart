import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';

class LanguagePicker extends StatelessWidget {
  final FormFieldValidator<String>? validator;
  final ValueChanged<String> onLanguageChanged;

  const LanguagePicker({required this.onLanguageChanged, this.validator, super.key});

  @override
  Widget build(BuildContext context) {
    final languageOptions = Languages.languages
      ..sort((a, b) {
        if (a.isoCode == 'de') return -1;
        return a.name.compareTo(b.name);
      });

    return DropdownButtonFormField(
      validator: validator,
      decoration: const InputDecoration(
        labelText: 'Language*',
        border: OutlineInputBorder(),
      ),
      items: languageOptions.map((language) {
        return DropdownMenuItem(
          value: language.name,
          child: Text(language.name),
        );
      }).toList(),
      onChanged: (String? newValue) {
        final selectedLanguage = languageOptions.firstWhere((language) => language.name == newValue);
        onLanguageChanged(selectedLanguage.isoCode);
      },
    );
  }
}
