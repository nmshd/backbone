import 'package:flutter/material.dart';

class InputField extends StatelessWidget {
  final void Function(String enteredText) onEnteredText;
  final String label;

  const InputField({required this.onEnteredText, required this.label, super.key});

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: .start,
      spacing: 8,
      children: [
        Text('$label:', style: const TextStyle(fontWeight: .bold)),
        SizedBox(
          width: 180,
          child: TextField(
            onChanged: onEnteredText,
            decoration: const InputDecoration(border: OutlineInputBorder()),
          ),
        ),
      ],
    );
  }
}
