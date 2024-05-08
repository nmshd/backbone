import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:watch_it/watch_it.dart';

import '../constants.dart';
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
      title: const Text('Settings'),
      content: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text('Theme', style: Theme.of(context).textTheme.bodyLarge),
          Gaps.h4,
          SegmentedButton(
            showSelectedIcon: false,
            segments: const [
              ButtonSegment(value: ThemeMode.light, icon: Icon(Icons.light_mode), label: Text('Light')),
              ButtonSegment(value: ThemeMode.system, icon: Icon(Icons.settings), label: Text('System')),
              ButtonSegment(value: ThemeMode.dark, icon: Icon(Icons.dark_mode), label: Text('Dark')),
            ],
            selected: {themeMode},
            onSelectionChanged: (selected) => GetIt.I<ThemeModeModel>().setThemeMode(selected.first),
          ),
        ],
      ),
      actions: [FilledButton(onPressed: () => context.pop(), child: const Text('Close'))],
    );
  }
}
