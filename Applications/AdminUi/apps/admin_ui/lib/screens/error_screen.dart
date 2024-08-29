import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '/core/core.dart';

class ErrorScreen extends StatelessWidget {
  final String errorMessage;

  const ErrorScreen({required this.errorMessage, super.key});

  @override
  Widget build(BuildContext context) {
    return Container(
      color: Colors.grey[200],
      child: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          crossAxisAlignment: CrossAxisAlignment.center,
          children: [
            Text(
              context.l10n.errorScreen_404NotFound,
              style: TextStyle(fontSize: 72, fontWeight: FontWeight.bold, color: Colors.grey[800], decoration: TextDecoration.none),
            ),
            Gaps.h16,
            Text(
              errorMessage,
              textAlign: TextAlign.center,
              style: const TextStyle(fontSize: 18, color: Colors.redAccent, decoration: TextDecoration.none),
            ),
            Gaps.h16,
            MouseRegion(
              cursor: SystemMouseCursors.click,
              child: GestureDetector(
                onTap: () => context.go('/identities'),
                child: Text(
                  context.l10n.errorScreen_goToHome,
                  style: TextStyle(fontSize: 16, color: Colors.blue, decoration: TextDecoration.none),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
