import 'package:flutter/material.dart';

class CustomElevatedButton extends StatelessWidget {
  final String text;
  final bool isEnabled;
  final VoidCallback? onPressed;
  final Color? backgroundColor;
  final Color? textColor;

  const CustomElevatedButton({
    required this.text,
    required this.isEnabled,
    this.onPressed,
    this.backgroundColor,
    this.textColor,
    super.key,
  });

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: double.infinity,
      height: 40,
      child: ElevatedButton(
        onPressed: isEnabled ? onPressed : null,
        style: ButtonStyle(
          backgroundColor: MaterialStatePropertyAll(
            isEnabled ? backgroundColor ?? Theme.of(context).colorScheme.primary : null,
          ),
        ),
        child: Text(
          text,
          style: TextStyle(color: isEnabled ? textColor ?? Colors.white : null),
        ),
      ),
    );
  }
}
