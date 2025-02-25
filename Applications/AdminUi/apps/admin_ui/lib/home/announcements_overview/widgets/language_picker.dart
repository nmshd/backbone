import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';

class LanguagePicker extends StatelessWidget {
  final FormFieldValidator<String>? validator;
  final String labelText;
  final TextEditingController? controller;
  final double? width;

  const LanguagePicker({
    required this.labelText,
    this.controller,
    this.validator,
    this.width,
    super.key,
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
        return DropdownMenu(
          controller: controller,
          errorText: fieldState.errorText,
          menuHeight: MediaQuery.of(context).size.height * 0.3,
          expandedInsets: EdgeInsets.zero,
          inputDecorationTheme: fieldState.hasError
              ? InputDecorationTheme(
                  border: OutlineInputBorder(borderSide: BorderSide(color: Theme.of(context).colorScheme.error)),
                  enabledBorder: OutlineInputBorder(borderSide: BorderSide(color: Theme.of(context).colorScheme.error)),
                )
              : const InputDecorationTheme(
                  border: OutlineInputBorder(),
                ),
          label: Text(
            labelText,
            style: fieldState.hasError ? TextStyle(color: Theme.of(context).colorScheme.error) : null,
          ),
          requestFocusOnTap: true,
          enableFilter: true,
          dropdownMenuEntries: languageOptions.map((language) {
            return DropdownMenuEntry(
              value: language.isoCode,
              label: language.name,
            );
          }).toList(),
        );
      },
    );
  }
}
