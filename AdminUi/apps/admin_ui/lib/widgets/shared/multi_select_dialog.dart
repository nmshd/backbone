import 'package:flutter/material.dart';
import 'package:multi_select_flutter/dialog/multi_select_dialog_field.dart';
import 'package:multi_select_flutter/util/multi_select_item.dart';

class MultiSelectDialog extends StatefulWidget {
  MultiSelectDialog(
    this.sendFilters, {
    required this.title,
    required this.selectedValues,
    required this.multiSelectItem,
    super.key,
  });

  final String title;
  final void Function() sendFilters;
  final List<MultiSelectItem<String>> multiSelectItem;
  late List<String> selectedValues;

  @override
  State<MultiSelectDialog> createState() => _MultiSelectDialogState();
}

class _MultiSelectDialogState extends State<MultiSelectDialog> {
  @override
  Widget build(BuildContext context) {
    return Container(
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
            widget.selectedValues.clear();
            widget.selectedValues.addAll(values);
          });
          widget.sendFilters();
        },
      ),
    );
  }
}
