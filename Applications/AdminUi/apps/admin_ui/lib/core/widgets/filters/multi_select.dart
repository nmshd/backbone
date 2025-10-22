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
  final List<String> selectedItems = [];

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      spacing: 8,
      children: [
        Text('${widget.label}:', style: const TextStyle(fontWeight: FontWeight.bold)),
        SizedBox(
          width: 250,
          child: DropdownButtonFormField2<String>(
            isExpanded: true,
            items: widget.options
                .map(
                  (item) => DropdownMenuItem(
                    value: item.value,
                    //disable default onTap to avoid closing menu when selecting an item
                    enabled: false,
                    child: StatefulBuilder(
                      builder: (context, menuSetState) {
                        final isSelected = selectedItems.contains(item.value);
                        return InkWell(
                          onTap: () {
                            isSelected ? selectedItems.remove(item.value) : selectedItems.add(item.value);
                            //This rebuilds the StatefulWidget to update the button's text
                            setState(() {});
                            //This rebuilds the dropdownMenu Widget to update the check mark
                            menuSetState(() {});

                            widget.onOptionSelected(selectedItems);
                          },
                          child: Container(
                            height: double.infinity,
                            padding: const EdgeInsets.symmetric(horizontal: 16),
                            child: Row(
                              children: [
                                if (isSelected) const Icon(Icons.check_box_outlined) else const Icon(Icons.check_box_outline_blank),
                                const SizedBox(width: 16),
                                Expanded(child: Text(item.label, style: const TextStyle(fontSize: 14))),
                              ],
                            ),
                          ),
                        );
                      },
                    ),
                  ),
                )
                .toList(),
            value: selectedItems.isEmpty ? null : selectedItems.last,
            onChanged: (value) {},
            selectedItemBuilder: (context) => widget.options
                .map(
                  (_) => Text(
                    widget.options.where((item) => selectedItems.contains(item.value)).map((item) => item.label).join(', '),
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                )
                .toList(),
            buttonStyleData: const ButtonStyleData(padding: EdgeInsets.zero),
            menuItemStyleData: const MenuItemStyleData(padding: EdgeInsets.zero),
            decoration: const InputDecoration(border: OutlineInputBorder()),
          ),
        ),
      ],
    );
  }
}
