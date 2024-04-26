import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

import '/core/constants.dart';

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
                if (selectedOperator!.userFriendlyOperator.isEmpty) return;
                widget.onFilterSelected(_operator = selectedOperator, _selectedDate);
              },
              items: FilterOperator.values.map((operator) {
                return DropdownMenuItem<FilterOperator>(
                  value: operator,
                  child: Text(operator.userFriendlyOperator),
                );
              }).toList(),
            ),
            Gaps.w8,
            InkWell(
              onTap: _selectNewDate,
              child: Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  border: Border.all(color: Colors.grey),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      _selectedDate != null ? DateFormat('yyyy-MM-dd').format(_selectedDate!) : 'Select date',
                      style: const TextStyle(fontSize: 14),
                    ),
                    Gaps.w8,
                    const Icon(Icons.calendar_today),
                    if (_selectedDate != null) ...[
                      Gaps.w8,
                      GestureDetector(
                        onTap: _clearDate,
                        child: const Icon(Icons.clear, size: 20),
                      ),
                    ],
                  ],
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
