// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';

class LanguagePicker extends StatelessWidget {
  final FormFieldValidator<String>? validator;
  final String labelText;
  final ValueChanged<String> onLanguageChanged;
  final TextEditingController? controller;

  const LanguagePicker({
    required this.labelText,
    required this.onLanguageChanged,
    super.key,
    this.validator,
    this.controller,
  });

  @override
  Widget build(BuildContext context) {
    final languageOptions = Languages.languages
      ..sort((a, b) {
        if (a.isoCode == 'de') return -1;
        return a.name.compareTo(b.name);
      });

    return DropdownMenu(
      // validator: validator,
      // decoration: InputDecoration(
      //   labelText: labelText,
      //   border: const OutlineInputBorder(),
      // ),
      controller: controller,
      dropdownMenuEntries: languageOptions.map((language) {
        return DropdownMenuEntry(
          value: language.isoCode,
          label: language.name,
        );
      }).toList(),
      onSelected: (String? newValue) {
        final selectedLanguage = languageOptions.firstWhere((language) => language.name == newValue);
        onLanguageChanged(selectedLanguage.isoCode);
      },
    );
  }
}
