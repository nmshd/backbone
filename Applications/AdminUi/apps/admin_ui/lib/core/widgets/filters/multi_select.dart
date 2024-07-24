import 'package:flutter/material.dart';
import 'package:multi_dropdown/multiselect_dropdown.dart';

import '/core/core.dart';

class MultiSelectFilter extends StatelessWidget {
  final String label;
  final String searchLabel;
  final MultiSelectController<String> controller;
  final void Function(List<ValueItem<String>> selectedOptions) onOptionSelected;

  const MultiSelectFilter({
    required this.label,
    required this.searchLabel,
    required this.controller,
    required this.onOptionSelected,
    super.key,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text('$label:', style: const TextStyle(fontWeight: FontWeight.bold)),
        Gaps.h8,
        SizedBox(
          width: 250,
          child: MultiSelectDropDown(
            hint: '',
            searchLabel: searchLabel,
            searchEnabled: true,
            controller: controller,
            options: controller.options,
            fieldBackgroundColor: Theme.of(context).colorScheme.surface,
            searchBackgroundColor: Theme.of(context).colorScheme.surface,
            dropdownBackgroundColor: Theme.of(context).colorScheme.surface,
            selectedOptionBackgroundColor: Theme.of(context).colorScheme.surface,
            selectedOptionTextColor: Theme.of(context).colorScheme.onSurface,
            optionsBackgroundColor: Theme.of(context).colorScheme.surface,
            optionTextStyle: TextStyle(color: Theme.of(context).colorScheme.onSurface),
            onOptionSelected: onOptionSelected,
          ),
        ),
      ],
    );
  }
}
