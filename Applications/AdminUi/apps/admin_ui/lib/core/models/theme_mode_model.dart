import 'dart:async';

import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';

class ThemeModeModel {
  static const String _themeModeKey = 'theme_mode';
  final SharedPreferences _sharedPreferences;

  final ValueNotifier<ThemeMode> themeMode;

  ThemeModeModel._(this._sharedPreferences, {required ThemeMode initialThemeMode}) : themeMode = ValueNotifier(initialThemeMode);

  static Future<ThemeModeModel> create() async {
    final sharedPreferences = await SharedPreferences.getInstance();

    final themeModeName = sharedPreferences.getString(_themeModeKey) ?? 'system';
    final loadedThemeMode = ThemeMode.values.byName(themeModeName);

    return ThemeModeModel._(sharedPreferences, initialThemeMode: loadedThemeMode);
  }

  void setThemeMode(ThemeMode newThemeMode) {
    themeMode.value = newThemeMode;
    unawaited(_sharedPreferences.setString(_themeModeKey, newThemeMode.name));
  }
}
