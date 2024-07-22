import 'package:flutter/material.dart';

class EntityDetails extends StatelessWidget {
  final String title;
  final String value;
  final VoidCallback? onIconPressed;
  final IconData? icon;
  final String? tooltipMessage;

  const EntityDetails({
    required this.title,
    required this.value,
    this.onIconPressed,
    this.icon,
    this.tooltipMessage,
  });

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
            TextSpan(text: value, style: Theme.of(context).textTheme.bodyLarge),
          ],
        ),
      ),
      onDeleted: onIconPressed,
      deleteIcon: Icon(icon),
      deleteButtonTooltipMessage: tooltipMessage,
    );
  }
}
