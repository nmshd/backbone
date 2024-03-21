import 'package:flutter/material.dart';

class DateFilter extends StatefulWidget {
  DateFilter(
    this.sendFilter, {
    required this.operator,
    required this.operators,
    required this.isDateSelected,
    required this.date,
    super.key,
  });

  final void Function() sendFilter;
  final List<String> operators;
  late bool isDateSelected;
  late DateTime date;
  String operator;

  @override
  State<DateFilter> createState() => _DateFilterState();
}

class _DateFilterState extends State<DateFilter> {
  Future<void> selectANewDate() async {
    final picked = await showDatePicker(
      context: context,
      initialDate: widget.date,
      firstDate: DateTime(2000),
      lastDate: DateTime(2101),
    );
    if (picked != null && picked != widget.date) {
      setState(() {
        widget
          ..date = picked
          ..isDateSelected = true;
      });
      widget.sendFilter();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      child: Row(
        children: [
          DropdownButton<String>(
            value: widget.operator,
            onChanged: (newValue) {
              setState(() {
                widget.operator = newValue!;
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
                          '${widget.date.year}-${widget.date.month}-${widget.date.day}',
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
