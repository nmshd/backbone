import 'package:flutter/material.dart';
import 'package:flutter/services.dart'; // Importing services.dart for TextInputFormatters

class NumberFilter extends StatefulWidget {
  NumberFilter(
    this.sendFilter, {
    required this.operators,
    required this.operator,
    required this.controller,
    required this.enteredValue,
    super.key,
  });

  final List<String> operators;
  final TextEditingController controller;
  late String operator;
  late String enteredValue;

  void Function() sendFilter;

  @override
  State<NumberFilter> createState() => _NumberFilterState();
}

class _NumberFilterState extends State<NumberFilter> {
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
          const SizedBox(
            width: 8,
          ),
          SizedBox(
            width: 50,
            height: 40,
            child: TextField(
              controller: widget.controller,
              onChanged: (value) {
                setState(() {
                  widget.enteredValue = value;
                  widget.sendFilter();
                });
              },
              decoration: const InputDecoration(
                border: OutlineInputBorder(),
              ),
              style: const TextStyle(fontSize: 12),
              inputFormatters: <TextInputFormatter>[
                FilteringTextInputFormatter.digitsOnly,
              ],
              keyboardType: TextInputType.number,
            ),
          ),
        ],
      ),
    );
  }
}
