import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

import '/core/constants.dart';
import 'to_filter_operator_dropdown_menu_item.dart';

class DateFilter extends StatefulWidget {
  final void Function(FilterOperator operator, DateTime? selectedDate) onFilterSelected;
  final String label;

  const DateFilter({required this.onFilterSelected, required this.label, super.key});

  @override
  State<DateFilter> createState() => _DateFilterState();
}

class _DateFilterState extends State<DateFilter> {
  FilterOperator _operator = FilterOperator.equal;
  DateTime? _selectedDate;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          '${widget.label}:',
          style: const TextStyle(fontWeight: FontWeight.bold),
        ),
        Gaps.h8,
        Row(
          children: [
            DropdownButton<FilterOperator>(
              value: _operator,
              onChanged: (selectedOperator) {
                if (selectedOperator == null) return;
                setState(() => _operator = selectedOperator);
                widget.onFilterSelected(selectedOperator, _selectedDate);
              },
              items: FilterOperator.values.toDropdownMenuItems(),
            ),
            Gaps.w8,
            SizedBox(
              width: 160,
              child: TextField(
                onTap: _selectNewDate,
                readOnly: true,
                controller: TextEditingController(
                  text: _selectedDate != null ? DateFormat('yyyy-MM-dd').format(_selectedDate!) : '',
                ),
                decoration: InputDecoration(
                  border: const OutlineInputBorder(),
                  suffixIcon:
                      _selectedDate == null ? const Icon(Icons.calendar_today) : IconButton(onPressed: _clearDate, icon: const Icon(Icons.clear)),
                ),
              ),
            ),
          ],
        ),
      ],
    );
  }

  void _clearDate() {
    setState(() => _selectedDate = null);
    widget.onFilterSelected(_operator, null);
  }

  Future<void> _selectNewDate() async {
    final picked = await showDatePicker(
      context: context,
      initialDate: _selectedDate ?? DateTime.now(),
      firstDate: DateTime(2000),
      lastDate: DateTime.now().add(const Duration(days: 1)),
    );

    if (picked == null) return;

    setState(() => _selectedDate = picked);
    widget.onFilterSelected(_operator, picked);
  }
}
