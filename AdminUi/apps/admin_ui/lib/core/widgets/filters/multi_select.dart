import 'package:flutter/material.dart';
import 'package:multi_dropdown/multiselect_dropdown.dart';

import '/core/core.dart';

class MultiSelectFilter extends StatelessWidget {
  final String label;
  final String searchLabel;
  final MultiSelectController<dynamic> controller;
  final void Function(List<ValueItem<dynamic>> selectedOptions) onOptionSelected;

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
        Text(
          '$label:',
          style: const TextStyle(fontWeight: FontWeight.bold),
        ),
        Gaps.h8,
        SizedBox(
          width: 200,
          child: MultiSelectDropDown(
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
          ),
        ),
      ],
    );
  }
}
