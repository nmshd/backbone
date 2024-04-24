import 'package:flutter/material.dart';

import '/core/constants.dart';

class DateFilter extends StatefulWidget {
  const DateFilter({
    required this.operators,
    required this.onDateSelected,
    required this.label,
    super.key,
  });

  final void Function(DateTime? selectedDate, String operator) onDateSelected;
  final List<String> operators;
  final String label;

  @override
  State<DateFilter> createState() => _DateFilterState();
}

class _DateFilterState extends State<DateFilter> {
  String _operator = '=';
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
            DropdownButton<String>(
              value: _operator,
              onChanged: (newValue) {
                setState(() => _operator = newValue!);
              },
              items: widget.operators.map((operator) {
                return DropdownMenuItem<String>(
                  value: operator,
                  child: Text(operator),
                );
              }).toList(),
            ),
            Gaps.w8,
            InkWell(
              onTap: _selectANewDate,
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
                      _selectedDate != null ? '${_selectedDate!.year}-${_selectedDate!.month}-${_selectedDate!.day}' : 'Select date',
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
    setState(() {
      _selectedDate = null;
      widget.onDateSelected(_selectedDate, _operator);
    });
  }

  Future<void> _selectANewDate() async {
    final picked = await showDatePicker(
      context: context,
      initialDate: _selectedDate ?? DateTime.now(),
      firstDate: DateTime(2000),
      lastDate: DateTime(2101),
    );
    if (picked != null) {
      setState(() {
        _selectedDate = picked;
        widget.onDateSelected(_selectedDate, _operator);
      });
    }
  }
}
