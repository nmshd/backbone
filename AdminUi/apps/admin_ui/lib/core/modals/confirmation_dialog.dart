import 'package:flutter/material.dart';

import '../extensions.dart';

Future<bool> showConfirmationDialog({
  required BuildContext context,
  required String title,
  required String message,
}) async {
  final result = await showDialog<bool>(
    context: context,
    builder: (BuildContext context) => _ConfirmationDialog(title: title, message: message),
  );

  return result ?? false;
}

class _ConfirmationDialog extends StatelessWidget {
  final String title;
  final String message;

  const _ConfirmationDialog({required this.title, required this.message});

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
      title: Text(title, textAlign: TextAlign.center),
      content: Padding(
        padding: const EdgeInsets.only(bottom: 32, top: 32),
        child: Text(message),
      ),
      actions: [
        OutlinedButton(
          onPressed: () => Navigator.of(context).pop(false),
          child: Text(context.l10n.cancel),
        ),
        FilledButton(
          onPressed: () => Navigator.of(context).pop(true),
          child: const Text('Remove'),
        ),
      ],
    );
  }
}
