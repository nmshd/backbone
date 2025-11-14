import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import 'to_filter_operator_dropdown_menu_item.dart';

class NumberFilter extends StatefulWidget {
  final void Function(FilterOperator operator, String enteredValue) onNumberSelected;
  final String label;

  const NumberFilter({required this.onNumberSelected, required this.label, super.key});

  @override
  State<NumberFilter> createState() => _NumberFilterState();
}

class _NumberFilterState extends State<NumberFilter> {
  late FilterOperator _operator = .equal;
  late String _value = '';

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: .start,
      spacing: 8,
      children: [
        Text('${widget.label}:', style: const TextStyle(fontWeight: .bold)),
        Row(
          spacing: 16,
          children: [
            DropdownButton<FilterOperator>(
              value: _operator,
              onChanged: (selectedOperator) {
                if (selectedOperator == null) return;
                setState(() => _operator = selectedOperator);
                widget.onNumberSelected(selectedOperator, _value);
              },
              items: FilterOperator.values.toDropdownMenuItems(),
            ),
            SizedBox(
              width: 120,
              child: TextField(
                onChanged: (enteredValue) {
                  _value = enteredValue;
                  widget.onNumberSelected(_operator, enteredValue);
                },
                decoration: const InputDecoration(border: OutlineInputBorder()),
                inputFormatters: <TextInputFormatter>[FilteringTextInputFormatter.digitsOnly],
                keyboardType: .number,
              ),
            ),
          ],
        ),
      ],
    );
  }
}
