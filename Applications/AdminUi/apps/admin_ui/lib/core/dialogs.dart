import 'package:flutter/material.dart';

Future<void> showLoadingDialog(BuildContext context, String text) async {
  await showDialog<void>(
    context: context,
    barrierDismissible: false,
    builder: (_) {
      return PopScope(
        canPop: false,
        child: Dialog(
          child: Padding(
            padding: const EdgeInsets.only(top: 32, bottom: 16, left: 16, right: 16),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              mainAxisSize: MainAxisSize.min,
              children: [
                const SizedBox(height: 60, width: 60, child: CircularProgressIndicator(strokeWidth: 12)),
                const SizedBox(height: 38),
                Text(text, style: Theme.of(context).textTheme.headlineSmall, textAlign: TextAlign.center),
              ],
            ),
          ),
        ),
      );
    },
  );
}
