import 'package:flutter/material.dart';
import 'package:watch_it/watch_it.dart';

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
      shape: RoundedRectangleBorder(borderRadius: .circular(8)),
      title: Text(context.l10n.settings, textAlign: .center),
      contentPadding: const EdgeInsets.only(left: 24, right: 24, top: 20, bottom: 32),
      content: Column(
        mainAxisSize: .min,
        crossAxisAlignment: .start,
        spacing: 4,
        children: [
          Text(context.l10n.theme, style: Theme.of(context).textTheme.bodyLarge),
          SegmentedButton<ThemeMode>(
            showSelectedIcon: false,
            segments: [
              ButtonSegment(value: .light, icon: const Icon(Icons.light_mode), label: Text(context.l10n.light)),
              ButtonSegment(value: .system, icon: const Icon(Icons.settings), label: Text(context.l10n.system)),
              ButtonSegment(value: .dark, icon: const Icon(Icons.dark_mode), label: Text(context.l10n.dark)),
            ],
            selected: {themeMode},
            onSelectionChanged: (selected) => GetIt.I<ThemeModeModel>().setThemeMode(selected.first),
          ),
        ],
      ),
      actions: [FilledButton(onPressed: () => Navigator.of(context, rootNavigator: true).pop(), child: Text(context.l10n.close))],
    );
  }
}
