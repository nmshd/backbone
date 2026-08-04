import 'package:dropdown_button2/dropdown_button2.dart';
import 'package:flutter/material.dart';

typedef MultiSelectFilterOption = ({String label, String value});

class MultiSelectFilter extends StatefulWidget {
  final String label;
  final List<MultiSelectFilterOption> options;
  final void Function(List<String> selectedOptions) onOptionSelected;

  const MultiSelectFilter({required this.label, required this.options, required this.onOptionSelected, super.key});

  @override
  State<MultiSelectFilter> createState() => _MultiSelectFilterState();
}

class _MultiSelectFilterState extends State<MultiSelectFilter> {
  final selectedItems = ValueNotifier<List<String>>([]);

  @override
  void dispose() {
    selectedItems.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: .start,
      spacing: 8,
      children: [
        Text('${widget.label}:', style: const TextStyle(fontWeight: .bold)),
        SizedBox(
          width: 250,
          child: DropdownButtonFormField2<String>(
            isExpanded: true,
            items: widget.options
                .map(
                  (item) => DropdownItem(
                    value: item.value,
                    closeOnTap: false,
                    child: ValueListenableBuilder(
                      valueListenable: selectedItems,
                      builder: (context, selectedItems, _) {
                        final isSelected = selectedItems.contains(item.value);
                        return Container(
                          height: double.infinity,
                          padding: const EdgeInsets.symmetric(horizontal: 16),
                          child: Row(
                            children: [
                              if (isSelected) const Icon(Icons.check_box_outlined) else const Icon(Icons.check_box_outline_blank),
                              const SizedBox(width: 16),
                              Expanded(child: Text(item.label, style: const TextStyle(fontSize: 14))),
                            ],
                          ),
                        );
                      },
                    ),
                  ),
                )
                .toList(),
            multiValueListenable: selectedItems,
            onChanged: (value) {
              final newSelection = selectedItems.value.contains(value) ? ([...selectedItems.value]..remove(value)) : [...selectedItems.value, value!];
              selectedItems.value = newSelection;
              widget.onOptionSelected(newSelection);
            },
            selectedItemBuilder: (context) => widget.options
                .map(
                  (_) => ValueListenableBuilder(
                    valueListenable: selectedItems,
                    builder: (context, selectedItems, _) => Text(
                      widget.options.where((item) => selectedItems.contains(item.value)).map((item) => item.label).join(', '),
                      maxLines: 1,
                      overflow: .ellipsis,
                    ),
                  ),
                )
                .toList(),
            buttonStyleData: const FormFieldButtonStyleData(padding: .zero),
            menuItemStyleData: const MenuItemStyleData(padding: .zero),
            decoration: const InputDecoration(border: OutlineInputBorder()),
          ),
        ),
      ],
    );
  }
}
