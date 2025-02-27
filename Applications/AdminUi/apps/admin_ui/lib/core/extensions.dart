import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:get_it/get_it.dart';

import '../generated/l10n/app_localizations.dart';
import 'theme/theme.dart';

extension AppLocalizationsExtension on BuildContext {
  AppLocalizations get l10n => AppLocalizations.of(this)!;
}

extension UnregisterIfRegistered on GetIt {
  Future<void> unregisterIfRegistered<T extends Object>() async {
    if (!isRegistered<T>()) return;

    await unregister<T>();
  }
}

extension GetCustomColors on BuildContext {
  CustomColors get customColors => Theme.of(this).extension<CustomColors>()!;
}

extension SetClipboardDataWithSnack on BuildContext {
  void setClipboardDataWithSuccessNotification({required String clipboardText, required String successMessage}) {
    Clipboard.setData(ClipboardData(text: clipboardText));

    ScaffoldMessenger.of(this).showSnackBar(SnackBar(content: Text(successMessage), showCloseIcon: true));
  }
}

extension Ellipsize on String {
  String ellipsize(int maxLength) {
    if (length <= maxLength) return this;

    return '${substring(0, maxLength - 3)}...';
  }
}
