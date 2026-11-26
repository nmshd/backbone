import 'package:material_ui/material_ui.dart';

import '../extensions.dart';

class CopyToClipboardButton extends StatelessWidget {
  final String clipboardText;
  final String successMessage;

  final ButtonStyle? style;
  final String? tooltip;

  const new({required this.clipboardText, required this.successMessage, this.style, this.tooltip, super.key});

  @override
  Widget build(BuildContext context) {
    return IconButton(
      icon: const Icon(Icons.copy),
      style: style,
      tooltip: tooltip,
      onPressed: () => context.setClipboardDataWithSuccessNotification(clipboardText: clipboardText, successMessage: successMessage),
    );
  }
}
