import 'package:enmeshed_ui_kit/enmeshed_ui_kit.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '/core/core.dart';

class ErrorScreen extends StatelessWidget {
  final String errorMessage;

  const ErrorScreen({required this.errorMessage, super.key});

  @override
  Widget build(BuildContext context) {
    return ColoredBox(
      color: Theme.of(context).colorScheme.tertiaryContainer,
      child: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text(
              '404',
              style: TextStyle(
                fontSize: 82,
                fontWeight: FontWeight.w400,
                color: Theme.of(context).colorScheme.onTertiaryContainer,
                decoration: TextDecoration.none,
              ),
            ),
            Text(
              context.l10n.errorScreen_notFound,
              style: TextStyle(
                fontSize: 30,
                fontWeight: FontWeight.w400,
                color: Theme.of(context).colorScheme.onTertiaryContainer,
                decoration: TextDecoration.none,
              ),
            ),
            Gaps.h16,
            Text(
              errorMessage,
              textAlign: TextAlign.center,
              style: TextStyle(fontSize: 18, color: Theme.of(context).colorScheme.tertiary, decoration: TextDecoration.none),
            ),
            Gaps.h16,
            FilledButton(
              onPressed: () => context.pop(),
              child: Text(context.l10n.errorScreen_goToHome, style: const TextStyle(fontSize: 16)),
            ),
          ],
        ),
      ),
    );
  }
}
