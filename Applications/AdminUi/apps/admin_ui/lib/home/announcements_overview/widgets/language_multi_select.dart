import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:dropdown_button2/dropdown_button2.dart';
import 'package:flutter/material.dart';

import '/core/core.dart';

class LanguageMultiSelect extends StatelessWidget {
  final List<String> selectedLanguages;
  final ValueChanged<List<String>> onSelectedLanguagesChanged;

  const LanguageMultiSelect({required this.selectedLanguages, required this.onSelectedLanguagesChanged, super.key});

  @override
  Widget build(BuildContext context) {
    final languageOptions = OptionalLanguageType.values.toList();

    return DropdownButtonFormField2<String>(
      isExpanded: true,
      hint: Text(context.l10n.languageMultiSelect_selectLanguages),
      items: languageOptions
          .map(
            (item) => DropdownMenuItem<String>(
              value: item.name,
              enabled: false,
              child: StatefulBuilder(
                builder: (context, menuSetState) {
                  final isSelected = selectedLanguages.contains(item.name);
                  return InkWell(
                    onTap: () {
                      isSelected ? selectedLanguages.remove(item.name) : selectedLanguages.add(item.name);
                      onSelectedLanguagesChanged(List.from(selectedLanguages));
                      menuSetState(() {});
                    },
                    child: Container(
                      height: double.infinity,
                      padding: const EdgeInsets.symmetric(horizontal: 16),
                      child: Row(
                        children: [
                          if (isSelected) const Icon(Icons.check_box_outlined) else const Icon(Icons.check_box_outline_blank),
                          const SizedBox(width: 16),
                          Expanded(
                            child: Text(
                              item.name,
                              style: const TextStyle(fontSize: 14),
                            ),
                          ),
                        ],
                      ),
                    ),
                  );
                },
              ),
            ),
          )
          .toList(),
      value: selectedLanguages.isEmpty ? null : selectedLanguages.last,
      onChanged: (value) {},
      selectedItemBuilder: (context) => languageOptions
          .map(
            (_) => Text(
              selectedLanguages.map((item) => item).join(', '),
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
            ),
          )
          .toList(),
      buttonStyleData: const ButtonStyleData(padding: EdgeInsets.zero),
      menuItemStyleData: const MenuItemStyleData(padding: EdgeInsets.zero),
      decoration: const InputDecoration(
        border: OutlineInputBorder(),
        contentPadding: EdgeInsets.symmetric(horizontal: 16, vertical: 12),
      ),
    );
  }
}
