import 'package:flutter/material.dart';

Future<bool> showConfirmationDialog({
  required BuildContext context,
  required String title,
  required String message,
  required String actionText,
  required String cancelActionText,
}) async {
  final result = await showDialog<bool>(
    context: context,
    builder: (BuildContext context) =>
        _ConfirmationDialog(title: title, message: message, actionText: actionText, cancelActionText: cancelActionText),
  );

  return result ?? false;
}

class _ConfirmationDialog extends StatelessWidget {
  final String title;
  final String message;
  final String actionText;
  final String cancelActionText;

  const _ConfirmationDialog({required this.title, required this.message, required this.actionText, required this.cancelActionText});

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
      title: Text(title, textAlign: TextAlign.center),
      contentPadding: const EdgeInsets.only(left: 24, right: 24, top: 20, bottom: 32),
      content: Text(message),
      actions: [
        OutlinedButton(
          onPressed: () => Navigator.of(context).pop(false),
          child: Text(cancelActionText),
        ),
        FilledButton(
          onPressed: () => Navigator.of(context).pop(true),
          child: Text(actionText),
        ),
      ],
    );
  }
}
