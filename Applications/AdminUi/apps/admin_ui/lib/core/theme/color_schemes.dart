import 'package:flex_seed_scheme/flex_seed_scheme.dart';
import 'package:flutter/material.dart';

import 'custom_colors.dart';

const _primarySeedColor = Color(0xFF17428D);
const _secondarySeedColor = Color(0xFF1A80D9);
const _tertiarySeedColor = Color(0xFFFF7600);
const _errorSeedColor = Color(0xFF8C1742);

final ThemeData lightTheme = _generateColorScheme(tonesConstructor: FlexTones.material, brightness: .light, customColors: lightCustomColors);
final ThemeData darkTheme = _generateColorScheme(tonesConstructor: FlexTones.material, brightness: .dark, customColors: darkCustomColors);

final ThemeData highContrastTheme = _generateColorScheme(
  tonesConstructor: FlexTones.ultraContrast,
  brightness: .light,
  customColors: lightHighContrastCustomColors,
);
final ThemeData highContrastDarkTheme = _generateColorScheme(
  tonesConstructor: FlexTones.ultraContrast,
  brightness: .dark,
  customColors: darkHighContrastCustomColors,
);

ThemeData _generateColorScheme({
  required FlexTones Function(Brightness brightness) tonesConstructor,
  required Brightness brightness,
  required ThemeExtension<dynamic> customColors,
  bool lightsOut = false,
}) {
  assert(!lightsOut || brightness == .dark, 'lightsOut can only be used with dark theme');

  final seededScheme = SeedColorScheme.fromSeeds(
    brightness: brightness,
    primaryKey: _primarySeedColor,
    secondaryKey: _secondarySeedColor,
    tertiaryKey: _tertiarySeedColor,
    errorKey: _errorSeedColor,
    surface: lightsOut ? Colors.black : null,
    tones: tonesConstructor(brightness),
  );

  // FlexSeedScheme 5 returns the standalone material_ui ColorScheme.
  // Copy every color role into the Flutter SDK type used by the app.
  final colorScheme = ColorScheme(
    brightness: seededScheme.brightness,
    primary: seededScheme.primary,
    onPrimary: seededScheme.onPrimary,
    primaryContainer: seededScheme.primaryContainer,
    onPrimaryContainer: seededScheme.onPrimaryContainer,
    primaryFixed: seededScheme.primaryFixed,
    primaryFixedDim: seededScheme.primaryFixedDim,
    onPrimaryFixed: seededScheme.onPrimaryFixed,
    onPrimaryFixedVariant: seededScheme.onPrimaryFixedVariant,
    secondary: seededScheme.secondary,
    onSecondary: seededScheme.onSecondary,
    secondaryContainer: seededScheme.secondaryContainer,
    onSecondaryContainer: seededScheme.onSecondaryContainer,
    secondaryFixed: seededScheme.secondaryFixed,
    secondaryFixedDim: seededScheme.secondaryFixedDim,
    onSecondaryFixed: seededScheme.onSecondaryFixed,
    onSecondaryFixedVariant: seededScheme.onSecondaryFixedVariant,
    tertiary: seededScheme.tertiary,
    onTertiary: seededScheme.onTertiary,
    tertiaryContainer: seededScheme.tertiaryContainer,
    onTertiaryContainer: seededScheme.onTertiaryContainer,
    tertiaryFixed: seededScheme.tertiaryFixed,
    tertiaryFixedDim: seededScheme.tertiaryFixedDim,
    onTertiaryFixed: seededScheme.onTertiaryFixed,
    onTertiaryFixedVariant: seededScheme.onTertiaryFixedVariant,
    error: seededScheme.error,
    onError: seededScheme.onError,
    errorContainer: seededScheme.errorContainer,
    onErrorContainer: seededScheme.onErrorContainer,
    surface: seededScheme.surface,
    onSurface: seededScheme.onSurface,
    surfaceDim: seededScheme.surfaceDim,
    surfaceBright: seededScheme.surfaceBright,
    surfaceContainerLowest: seededScheme.surfaceContainerLowest,
    surfaceContainerLow: seededScheme.surfaceContainerLow,
    surfaceContainer: seededScheme.surfaceContainer,
    surfaceContainerHigh: seededScheme.surfaceContainerHigh,
    surfaceContainerHighest: seededScheme.surfaceContainerHighest,
    onSurfaceVariant: seededScheme.onSurfaceVariant,
    outline: seededScheme.outline,
    outlineVariant: seededScheme.outlineVariant,
    shadow: seededScheme.shadow,
    scrim: seededScheme.scrim,
    inverseSurface: seededScheme.inverseSurface,
    onInverseSurface: seededScheme.onInverseSurface,
    inversePrimary: seededScheme.inversePrimary,
    surfaceTint: seededScheme.surfaceTint,
  );

  final cardTheme = CardThemeData(color: colorScheme.surface, shadowColor: colorScheme.shadow, surfaceTintColor: colorScheme.surfaceTint);

  return ThemeData(colorScheme: colorScheme, extensions: [customColors], cardTheme: cardTheme);
}
