import 'package:flutter/material.dart';

class EntityDetails extends StatelessWidget {
  final String title;
  final String value;

  const EntityDetails({required this.title, required this.value, super.key});

  @override
  Widget build(BuildContext context) {
    return RawChip(
      label: Text.rich(
        TextSpan(
          children: [
            TextSpan(text: '$title ', style: Theme.of(context).textTheme.bodyLarge!.copyWith(fontWeight: FontWeight.bold)),
            TextSpan(text: value, style: Theme.of(context).textTheme.bodyLarge),
          ],
        ),
      ),
    );
  }
}
