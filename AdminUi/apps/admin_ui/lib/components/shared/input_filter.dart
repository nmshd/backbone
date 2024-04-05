import 'package:flutter/material.dart';

class InputField extends StatefulWidget {
  const InputField({required this.onEnteredText, required this.title, super.key});

  final void Function(String enteredText) onEnteredText;
  final String title;

  @override
  State<InputField> createState() => _InputFieldState();
}

class _InputFieldState extends State<InputField> {
  final _searchAddressController = TextEditingController();

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 180,
      height: 70,
      child: TextField(
        controller: _searchAddressController,
        onChanged: (value) {
          setState(() => widget.onEnteredText(value));
        },
        decoration: InputDecoration(
          labelText: widget.title,
          border: const OutlineInputBorder(),
        ),
      ),
    );
  }
}
