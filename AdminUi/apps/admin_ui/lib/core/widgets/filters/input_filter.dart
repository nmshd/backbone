import 'package:flutter/material.dart';

import '/core/core.dart';

class InputField extends StatefulWidget {
  const InputField({required this.onEnteredText, required this.label, super.key});

  final void Function(String enteredText) onEnteredText;
  final String label;

  @override
  State<InputField> createState() => _InputFieldState();
}

class _InputFieldState extends State<InputField> {
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
        SizedBox(
          width: 180,
          child: TextField(
            onChanged: (value) {
              setState(() => widget.onEnteredText(value));
            },
            decoration: const InputDecoration(border: OutlineInputBorder()),
          ),
        ),
      ],
    );
  }
}
