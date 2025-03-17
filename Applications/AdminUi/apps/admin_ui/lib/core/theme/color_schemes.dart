import 'package:flex_seed_scheme/flex_seed_scheme.dart';
import 'package:flutter/material.dart';

import 'custom_colors.dart';

const _primarySeedColor = Color(0xFF17428D);
const _secondarySeedColor = Color(0xFF1A80D9);
const _tertiarySeedColor = Color(0xFFFF7600);
const _errorSeedColor = Color(0xFF8C1742);

final lightTheme = _generateColorScheme(tonesConstructor: FlexTones.material, brightness: Brightness.light, customColors: lightCustomColors);
final darkTheme = _generateColorScheme(tonesConstructor: FlexTones.material, brightness: Brightness.dark, customColors: darkCustomColors);

final highContrastTheme = _generateColorScheme(
  tonesConstructor: FlexTones.ultraContrast,
  brightness: Brightness.light,
  customColors: lightHighContrastCustomColors,
);
final highContrastDarkTheme = _generateColorScheme(
  tonesConstructor: FlexTones.ultraContrast,
  brightness: Brightness.dark,
  customColors: darkHighContrastCustomColors,
);

ThemeData _generateColorScheme({
  required FlexTones Function(Brightness brightness) tonesConstructor,
  required Brightness brightness,
  required ThemeExtension<dynamic> customColors,
  bool lightsOut = false,
}) {
  assert(!lightsOut || brightness == Brightness.dark, 'lightsOut can only be used with dark theme');

  final colorScheme = SeedColorScheme.fromSeeds(
    brightness: brightness,
    primaryKey: _primarySeedColor,
    secondaryKey: _secondarySeedColor,
    tertiaryKey: _tertiarySeedColor,
    errorKey: _errorSeedColor,
    surface: lightsOut ? Colors.black : null,
    tones: tonesConstructor(brightness),
  );

  final cardTheme = CardTheme(color: colorScheme.surface, shadowColor: colorScheme.shadow, surfaceTintColor: colorScheme.surfaceTint);

  return ThemeData(colorScheme: colorScheme, extensions: [customColors], cardTheme: cardTheme);
}
