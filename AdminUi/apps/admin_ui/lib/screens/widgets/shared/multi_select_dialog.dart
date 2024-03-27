import 'package:flutter/material.dart';
import 'package:multi_select_flutter/multi_select_flutter.dart';

class MultiSelectDialog extends StatefulWidget {
  const MultiSelectDialog(
    this.sendFilters, {
    required this.title,
    required this.onSelectedValues,
    required this.multiSelectItem,
    super.key,
  });

  final String title;
  final void Function() sendFilters;
  final List<MultiSelectItem<String>> multiSelectItem;
  final void Function(List<String> selectedValues) onSelectedValues;

  @override
  State<MultiSelectDialog> createState() => _MultiSelectDialogState();
}

class _MultiSelectDialogState extends State<MultiSelectDialog> {
  late List<String> selectedValues = [];

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 125,
      child: MultiSelectDialogField(
        searchable: true,
        dialogHeight: 200,
        dialogWidth: 200,
        items: widget.multiSelectItem,
        title: Text(widget.title),
        selectedColor: Colors.blue,
        buttonText: Text('${widget.title}s'),
        onConfirm: (values) {
          setState(() {
            selectedValues
              ..clear()
              ..addAll(values);

            widget.onSelectedValues(selectedValues);
          });
          widget.sendFilters();
        },
      ),
    );
  }
}
