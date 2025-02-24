import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';

class LanguagePicker extends StatelessWidget {
  final FormFieldValidator<String>? validator;
  final List<String> selectedLanguages;
  final ValueChanged<List<String>> onSelectedLanguagesChanged;

  const LanguagePicker({required this.selectedLanguages, required this.onSelectedLanguagesChanged, this.validator, super.key});

  @override
  Widget build(BuildContext context) {
    final languageOptions = OptionalLanguageType.values.toList();

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
      onChanged: (String? newValue) {},
    );
  }
}
