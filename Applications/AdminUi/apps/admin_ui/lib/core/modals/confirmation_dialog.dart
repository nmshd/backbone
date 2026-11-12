import 'package:go_router/go_router.dart';
import 'package:material_ui/material_ui.dart';

import '../extensions.dart';

Future<bool> showConfirmationDialog({
  required BuildContext context,
  required String title,
  required String message,
  required String actionText,
  String? cancelActionText,
}) async {
  final result = await showDialog<bool>(
    context: context,
    builder: (context) => _ConfirmationDialog(title: title, message: message, actionText: actionText, cancelActionText: cancelActionText),
  );

  return result ?? false;
}

class _ConfirmationDialog extends StatelessWidget {
  final String title;
  final String message;
  final String actionText;
  final String? cancelActionText;

  const new({required this.title, required this.message, required this.actionText, this.cancelActionText});

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      shape: RoundedRectangleBorder(borderRadius: .circular(8)),
      title: Text(title, textAlign: .center),
      contentPadding: const EdgeInsets.only(left: 24, right: 24, top: 20, bottom: 32),
      content: Text(message),
      actions: [
        OutlinedButton(onPressed: () => context.pop(false), child: Text(cancelActionText ?? context.l10n.cancel)),
        FilledButton(onPressed: () => context.pop(true), child: Text(actionText)),
      ],
    );
  }
}
