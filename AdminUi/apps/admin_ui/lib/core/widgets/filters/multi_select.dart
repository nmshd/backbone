import 'package:flutter/material.dart';
import 'package:multi_dropdown/multiselect_dropdown.dart';

import '/core/core.dart';

class MultiSelectFilter extends StatelessWidget {
  final String label;
  final String searchLabel;
  final MultiSelectController<String> controller;
  final void Function(List<ValueItem<String>> selectedOptions) onOptionSelected;
  final void Function(int index, ValueItem<String> option) onOptionRemoved;

  const MultiSelectFilter({
    required this.label,
    required this.searchLabel,
    required this.controller,
    required this.onOptionSelected,
    required this.onOptionRemoved,
    super.key,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          '$label:',
          style: const TextStyle(fontWeight: FontWeight.bold),
        ),
        Gaps.h8,
        SizedBox(
          width: 250,
          child: MultiSelectDropDown(
            hint: '',
            searchLabel: searchLabel,
            searchEnabled: true,
            controller: controller,
            options: controller.options,
            fieldBackgroundColor: Theme.of(context).colorScheme.background,
            searchBackgroundColor: Theme.of(context).colorScheme.background,
            dropdownBackgroundColor: Theme.of(context).colorScheme.background,
            selectedOptionBackgroundColor: Theme.of(context).colorScheme.background,
            selectedOptionTextColor: Theme.of(context).colorScheme.onBackground,
            optionsBackgroundColor: Theme.of(context).colorScheme.background,
            optionTextStyle: TextStyle(color: Theme.of(context).colorScheme.onBackground),
            onOptionSelected: onOptionSelected,
            onOptionRemoved: onOptionRemoved,
          ),
        ),
      ],
    );
  }
}
