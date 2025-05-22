import 'package:enmeshed_ui_kit/enmeshed_ui_kit.dart';
import 'package:flutter/material.dart';

class InputField extends StatelessWidget {
  final void Function(String enteredText) onEnteredText;
  final String label;

  const InputField({required this.onEnteredText, required this.label, super.key});

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text('$label:', style: const TextStyle(fontWeight: FontWeight.bold)),
        Gaps.h8,
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
