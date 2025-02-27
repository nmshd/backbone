import 'package:flutter/material.dart';

import '/core/core.dart';

class CopyableEntityDetails extends StatelessWidget {
  final String title;
  final String value;
  final int? ellipsize;

  const CopyableEntityDetails({required this.title, required this.value, this.ellipsize, super.key});

  @override
  Widget build(BuildContext context) {
    return EntityDetails(
      title: title,
      value: value,
      icon: Icons.copy,
      tooltipMessage: context.l10n.copyToClipboard_tooltip,
      onIconPressed:
          () => context.setClipboardDataWithSuccessNotification(clipboardText: value, successMessage: context.l10n.copyToClipboard_success(title)),
      ellipsize: ellipsize,
    );
  }
}

class EntityDetails extends StatelessWidget {
  final String title;
  final String value;
  final VoidCallback? onIconPressed;
  final IconData? icon;
  final String? tooltipMessage;
  final int? ellipsize;

  const EntityDetails({required this.title, required this.value, this.onIconPressed, this.icon, this.tooltipMessage, this.ellipsize, super.key});

  @override
  Widget build(BuildContext context) {
    assert(
      onIconPressed == null || (onIconPressed != null && icon != null || tooltipMessage != null),
      'If edit is provided, icon and tooltipMessage must be provided too.',
    );

    return RawChip(
      label: Text.rich(
        TextSpan(
          children: [
            TextSpan(text: '$title ', style: Theme.of(context).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.bold)),
            TextSpan(text: ellipsize != null ? value.ellipsize(ellipsize!) : value, style: Theme.of(context).textTheme.bodyLarge),
          ],
        ),
      ),
      onDeleted: onIconPressed,
      deleteIcon: Icon(icon),
      deleteButtonTooltipMessage: tooltipMessage,
    );
  }
}
