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
  late String operator = '=';
  DateTime? selectedDate;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          '${widget.label}:',
          style: const TextStyle(fontWeight: FontWeight.bold),
        ),
        Row(
          children: [
            DropdownButton<String>(
              value: operator,
              onChanged: (newValue) {
                setState(() => operator = newValue!);
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
                      selectedDate != null ? '${selectedDate!.year}-${selectedDate!.month}-${selectedDate!.day}' : 'Select date',
                      style: const TextStyle(fontSize: 14),
                    ),
                    Gaps.w8,
                    const Icon(Icons.calendar_today),
                    if (selectedDate != null) ...[
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
      selectedDate = null;
    });
    widget.onDateSelected(selectedDate, operator);
  }

  Future<void> _selectANewDate() async {
    final picked = await showDatePicker(
      context: context,
      initialDate: selectedDate ?? DateTime.now(),
      firstDate: DateTime(2000),
      lastDate: DateTime(2101),
    );
    if (picked != null) {
      setState(() {
        selectedDate = picked;
        widget.onDateSelected(selectedDate, operator);
      });
    }
  }
}
