import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

import 'to_filter_operator_dropdown_menu_item.dart';

class DateFilter extends StatefulWidget {
  final void Function(FilterOperator operator, DateTime? selectedDate) onFilterSelected;
  final String label;

  const DateFilter({required this.onFilterSelected, required this.label, super.key});

  @override
  State<DateFilter> createState() => _DateFilterState();
}

class _DateFilterState extends State<DateFilter> {
  late final TextEditingController _controller;
  FilterOperator _operator = .equal;
  DateTime? _selectedDate;

  @override
  void initState() {
    super.initState();

    _controller = TextEditingController();
  }

  @override
  void dispose() {
    _controller.dispose();

    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: .start,
      spacing: 8,
      children: [
        Text('${widget.label}:', style: const TextStyle(fontWeight: .bold)),
        Row(
          spacing: 8,
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
            SizedBox(
              width: 160,
              child: TextField(
                onTap: _selectNewDate,
                readOnly: true,
                canRequestFocus: false,
                controller: _controller,
                decoration: InputDecoration(
                  border: const OutlineInputBorder(),
                  suffixIcon: _selectedDate == null
                      ? const Icon(Icons.calendar_today)
                      : IconButton(onPressed: _clearDate, icon: const Icon(Icons.clear)),
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
    _controller.text = '';
    widget.onFilterSelected(_operator, null);
  }

  Future<void> _selectNewDate() async {
    final picked = await showDatePicker(
      context: context,
      initialDate: _selectedDate ?? .now(),
      firstDate: .new(2000),
      lastDate: .now(),
      locale: Localizations.localeOf(context),
    );

    if (picked == null || !mounted) return;

    setState(() => _selectedDate = picked);
    _controller.text = DateFormat.yMd(Localizations.localeOf(context).languageCode).format(picked);
    widget.onFilterSelected(_operator, picked);
  }
}
