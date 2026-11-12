import 'package:admin_ui/core/theme/theme.dart';
import 'package:flex_seed_scheme/flex_seed_scheme.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  for (final (name, theme, brightness, tones) in [
    ('light', lightTheme, Brightness.light, FlexTones.material(Brightness.light)),
    ('dark', darkTheme, Brightness.dark, FlexTones.material(Brightness.dark)),
    ('high contrast light', highContrastTheme, Brightness.light, FlexTones.ultraContrast(Brightness.light)),
    ('high contrast dark', highContrastDarkTheme, Brightness.dark, FlexTones.ultraContrast(Brightness.dark)),
  ]) {
    test('$name theme preserves all generated color roles', () {
      final expected = SeedColorScheme.fromSeeds(
        brightness: brightness,
        primaryKey: const Color(0xFF17428D),
        secondaryKey: const Color(0xFF1A80D9),
        tertiaryKey: const Color(0xFFFF7600),
        errorKey: const Color(0xFF8C1742),
        tones: tones,
      );
      final actual = theme.colorScheme;

      expect(actual.brightness, brightness);
      expect(actual.primary, expected.primary, reason: 'primary');
      expect(actual.onPrimary, expected.onPrimary, reason: 'onPrimary');
      expect(actual.primaryContainer, expected.primaryContainer, reason: 'primaryContainer');
      expect(actual.onPrimaryContainer, expected.onPrimaryContainer, reason: 'onPrimaryContainer');
      expect(actual.primaryFixed, expected.primaryFixed, reason: 'primaryFixed');
      expect(actual.primaryFixedDim, expected.primaryFixedDim, reason: 'primaryFixedDim');
      expect(actual.onPrimaryFixed, expected.onPrimaryFixed, reason: 'onPrimaryFixed');
      expect(actual.onPrimaryFixedVariant, expected.onPrimaryFixedVariant, reason: 'onPrimaryFixedVariant');
      expect(actual.secondary, expected.secondary, reason: 'secondary');
      expect(actual.onSecondary, expected.onSecondary, reason: 'onSecondary');
      expect(actual.secondaryContainer, expected.secondaryContainer, reason: 'secondaryContainer');
      expect(actual.onSecondaryContainer, expected.onSecondaryContainer, reason: 'onSecondaryContainer');
      expect(actual.secondaryFixed, expected.secondaryFixed, reason: 'secondaryFixed');
      expect(actual.secondaryFixedDim, expected.secondaryFixedDim, reason: 'secondaryFixedDim');
      expect(actual.onSecondaryFixed, expected.onSecondaryFixed, reason: 'onSecondaryFixed');
      expect(actual.onSecondaryFixedVariant, expected.onSecondaryFixedVariant, reason: 'onSecondaryFixedVariant');
      expect(actual.tertiary, expected.tertiary, reason: 'tertiary');
      expect(actual.onTertiary, expected.onTertiary, reason: 'onTertiary');
      expect(actual.tertiaryContainer, expected.tertiaryContainer, reason: 'tertiaryContainer');
      expect(actual.onTertiaryContainer, expected.onTertiaryContainer, reason: 'onTertiaryContainer');
      expect(actual.tertiaryFixed, expected.tertiaryFixed, reason: 'tertiaryFixed');
      expect(actual.tertiaryFixedDim, expected.tertiaryFixedDim, reason: 'tertiaryFixedDim');
      expect(actual.onTertiaryFixed, expected.onTertiaryFixed, reason: 'onTertiaryFixed');
      expect(actual.onTertiaryFixedVariant, expected.onTertiaryFixedVariant, reason: 'onTertiaryFixedVariant');
      expect(actual.error, expected.error, reason: 'error');
      expect(actual.onError, expected.onError, reason: 'onError');
      expect(actual.errorContainer, expected.errorContainer, reason: 'errorContainer');
      expect(actual.onErrorContainer, expected.onErrorContainer, reason: 'onErrorContainer');
      expect(actual.surface, expected.surface, reason: 'surface');
      expect(actual.onSurface, expected.onSurface, reason: 'onSurface');
      expect(actual.surfaceDim, expected.surfaceDim, reason: 'surfaceDim');
      expect(actual.surfaceBright, expected.surfaceBright, reason: 'surfaceBright');
      expect(actual.surfaceContainerLowest, expected.surfaceContainerLowest, reason: 'surfaceContainerLowest');
      expect(actual.surfaceContainerLow, expected.surfaceContainerLow, reason: 'surfaceContainerLow');
      expect(actual.surfaceContainer, expected.surfaceContainer, reason: 'surfaceContainer');
      expect(actual.surfaceContainerHigh, expected.surfaceContainerHigh, reason: 'surfaceContainerHigh');
      expect(actual.surfaceContainerHighest, expected.surfaceContainerHighest, reason: 'surfaceContainerHighest');
      expect(actual.onSurfaceVariant, expected.onSurfaceVariant, reason: 'onSurfaceVariant');
      expect(actual.outline, expected.outline, reason: 'outline');
      expect(actual.outlineVariant, expected.outlineVariant, reason: 'outlineVariant');
      expect(actual.shadow, expected.shadow, reason: 'shadow');
      expect(actual.scrim, expected.scrim, reason: 'scrim');
      expect(actual.inverseSurface, expected.inverseSurface, reason: 'inverseSurface');
      expect(actual.onInverseSurface, expected.onInverseSurface, reason: 'onInverseSurface');
      expect(actual.inversePrimary, expected.inversePrimary, reason: 'inversePrimary');
      expect(actual.surfaceTint, expected.surfaceTint, reason: 'surfaceTint');
      expect(theme.cardTheme.surfaceTintColor, expected.surfaceTint);
      expect(theme.extension<CustomColors>(), isNotNull);
    });

    testWidgets('$name theme renders SDK Material widgets', (tester) async {
      await tester.pumpWidget(
        MaterialApp(
          theme: theme,
          home: const Scaffold(body: Card(child: Text('Theme preview'))),
        ),
      );

      expect(find.text('Theme preview'), findsOneWidget);
      expect(tester.takeException(), isNull);
    });
  }
}
