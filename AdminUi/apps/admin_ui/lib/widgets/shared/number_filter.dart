import 'package:flutter/material.dart';
import 'package:flutter/services.dart'; // Importing services.dart for TextInputFormatters

class NumberFilter extends StatefulWidget {
  const NumberFilter({
    required this.operators,
    required this.onNumberSelected,
    super.key,
  });

  final List<String> operators;
  final void Function(String operator, String enteredValue) onNumberSelected;

  @override
  State<NumberFilter> createState() => _NumberFilterState();
}

class _NumberFilterState extends State<NumberFilter> {
  late TextEditingController controller = TextEditingController();
  late String operator = '=';
  late String enteredValue = '';

  @override
  Widget build(BuildContext context) {
    return Row(
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
        const SizedBox(
          width: 8,
        ),
        SizedBox(
          width: 50,
          height: 40,
          child: TextField(
            controller: controller,
            onChanged: (value) {
              setState(() {
                enteredValue = value;
                widget.onNumberSelected(operator, enteredValue);
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
    );
  }
}
