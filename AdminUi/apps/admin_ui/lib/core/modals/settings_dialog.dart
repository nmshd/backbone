import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:watch_it/watch_it.dart';

import '../constants.dart';
import '../extensions.dart';
import '../models/models.dart';

Future<void> openSettingsDialog(BuildContext context) async {
  await showDialog<void>(context: context, builder: (_) => const _SettingsDialog());
}

class _SettingsDialog extends StatelessWidget with WatchItMixin {
  const _SettingsDialog();

  @override
  Widget build(BuildContext context) {
    final themeMode = watchValue((ThemeModeModel x) => x.themeMode);

    return AlertDialog(
      title: Text(context.l10n.settings),
      content: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(context.l10n.theme, style: Theme.of(context).textTheme.bodyLarge),
          Gaps.h4,
          SegmentedButton(
            showSelectedIcon: false,
            segments: [
              ButtonSegment(value: ThemeMode.light, icon: const Icon(Icons.light_mode), label: Text(context.l10n.light)),
              ButtonSegment(value: ThemeMode.system, icon: const Icon(Icons.settings), label: Text(context.l10n.system)),
              ButtonSegment(value: ThemeMode.dark, icon: const Icon(Icons.dark_mode), label: Text(context.l10n.dark)),
            ],
            selected: {themeMode},
            onSelectionChanged: (selected) => GetIt.I<ThemeModeModel>().setThemeMode(selected.first),
          ),
        ],
      ),
      actions: [FilledButton(onPressed: () => context.pop(), child: Text(context.l10n.close))],
    );
  }
}
