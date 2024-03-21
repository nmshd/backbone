import 'package:flutter/material.dart';

class DateFilter extends StatefulWidget {
  DateFilter({
    required this.operators,
    required this.onDateSelected,
    super.key,
  });

  final void Function(DateTime selectedDate, String operator, bool isDateSelected) onDateSelected;
  final List<String> operators;

  @override
  State<DateFilter> createState() => _DateFilterState();
}

class _DateFilterState extends State<DateFilter> {
  late String operator = '=';
  late DateTime selectedDate = DateTime.now();
  late bool isDateSelected = false;

  Future<void> selectANewDate() async {
    final picked = await showDatePicker(
      context: context,
      initialDate: selectedDate,
      firstDate: DateTime(2000),
      lastDate: DateTime(2101),
    );
    if (picked != null) {
      setState(() {
        selectedDate = picked;
        isDateSelected = true;
        widget.onDateSelected(selectedDate, operator, isDateSelected);
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      child: Row(
        children: [
          DropdownButton<String>(
            value: operator,
            onChanged: (newValue) {
              setState(() {
                operator = newValue!;
              });
            },
            items: widget.operators.map((operator) {
              return DropdownMenuItem<String>(
                value: operator,
                child: Text(operator),
              );
            }).toList(),
          ),
          Container(
            margin: const EdgeInsets.symmetric(horizontal: 8),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const SizedBox(height: 8),
                InkWell(
                  onTap: selectANewDate,
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
                          '${selectedDate.year}-${selectedDate.month}-${selectedDate.day}',
                          style: const TextStyle(fontSize: 16),
                        ),
                        const Icon(Icons.calendar_today),
                      ],
                    ),
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
