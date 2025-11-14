import 'package:flex_seed_scheme/flex_seed_scheme.dart';
import 'package:flutter/material.dart';

const _successSeedColor = Color(0xFF428C17);
const _warningSeedColor = Color(0xFF8C6117);
const _decorativeSeedColor = Color(0xFF61178C);
const _decorative2SeedColor = Color(0xFF8EB0E9);

final lightCustomColors = CustomColors.generate(
  brightness: .light,
  tonesConstructor: FlexTones.material,
  successSeedColor: _successSeedColor,
  warningSeedColor: _warningSeedColor,
  decorativeSeedColor: _decorativeSeedColor,
  decorative2SeedColor: _decorative2SeedColor,
);

final darkCustomColors = CustomColors.generate(
  brightness: .dark,
  tonesConstructor: FlexTones.material,
  successSeedColor: _successSeedColor,
  warningSeedColor: _warningSeedColor,
  decorativeSeedColor: _decorativeSeedColor,
  decorative2SeedColor: _decorative2SeedColor,
);

final lightHighContrastCustomColors = CustomColors.generate(
  brightness: .light,
  tonesConstructor: FlexTones.ultraContrast,
  successSeedColor: _successSeedColor,
  warningSeedColor: _warningSeedColor,
  decorativeSeedColor: _decorativeSeedColor,
  decorative2SeedColor: _decorative2SeedColor,
);

final darkHighContrastCustomColors = CustomColors.generate(
  brightness: .dark,
  tonesConstructor: FlexTones.ultraContrast,
  successSeedColor: _successSeedColor,
  warningSeedColor: _warningSeedColor,
  decorativeSeedColor: _decorativeSeedColor,
  decorative2SeedColor: _decorative2SeedColor,
);

@immutable
class CustomColors extends ThemeExtension<CustomColors> {
  const CustomColors({
    required this.success,
    required this.onSuccess,
    required this.successContainer,
    required this.onSuccessContainer,
    required this.warningFixed,
    required this.warning,
    required this.onWarning,
    required this.warningContainer,
    required this.onWarningContainer,
    required this.decorative,
    required this.onDecorative,
    required this.decorativeContainer,
    required this.onDecorativeContainer,
    required this.decorative2,
    required this.onDecorative2,
    required this.decorative2Container,
    required this.onDecorative2Container,
  });

  factory CustomColors.generate({
    required FlexTones Function(Brightness brightness) tonesConstructor,
    required Brightness brightness,
    required Color successSeedColor,
    required Color warningSeedColor,
    required Color decorativeSeedColor,
    required Color decorative2SeedColor,
  }) {
    final scheme = SeedColorScheme.fromSeeds(
      brightness: brightness,
      primaryKey: successSeedColor,
      secondaryKey: warningSeedColor,
      tertiaryKey: decorativeSeedColor,
      errorKey: decorative2SeedColor,
      tones: tonesConstructor(brightness),
    );

    return CustomColors(
      success: scheme.primary,
      onSuccess: scheme.onPrimary,
      successContainer: scheme.primaryContainer,
      onSuccessContainer: scheme.onPrimaryContainer,
      warningFixed: warningSeedColor,
      warning: scheme.secondary,
      onWarning: scheme.onSecondary,
      warningContainer: scheme.secondaryContainer,
      onWarningContainer: scheme.onSecondaryContainer,
      decorative: scheme.tertiary,
      onDecorative: scheme.onTertiary,
      decorativeContainer: scheme.tertiaryContainer,
      onDecorativeContainer: scheme.onTertiaryContainer,
      decorative2: scheme.error,
      onDecorative2: scheme.onError,
      decorative2Container: scheme.errorContainer,
      onDecorative2Container: scheme.onErrorContainer,
    );
  }

  final Color success;
  final Color onSuccess;
  final Color successContainer;
  final Color onSuccessContainer;
  final Color warningFixed;
  final Color warning;
  final Color onWarning;
  final Color warningContainer;
  final Color onWarningContainer;
  final Color decorative;
  final Color onDecorative;
  final Color decorativeContainer;
  final Color onDecorativeContainer;
  final Color decorative2;
  final Color onDecorative2;
  final Color decorative2Container;
  final Color onDecorative2Container;

  @override
  CustomColors copyWith({
    Color? success,
    Color? onSuccess,
    Color? successContainer,
    Color? onSuccessContainer,
    Color? warningFixed,
    Color? warning,
    Color? onWarning,
    Color? warningContainer,
    Color? onWarningContainer,
    Color? decorative,
    Color? onDecorative,
    Color? decorativeContainer,
    Color? onDecorativeContainer,
    Color? decorative2,
    Color? onDecorative2,
    Color? decorative2Container,
    Color? onDecorative2Container,
  }) {
    return CustomColors(
      success: success ?? this.success,
      onSuccess: onSuccess ?? this.onSuccess,
      successContainer: successContainer ?? this.successContainer,
      onSuccessContainer: onSuccessContainer ?? this.onSuccessContainer,
      warningFixed: warningFixed ?? this.warningFixed,
      warning: warning ?? this.warning,
      onWarning: onWarning ?? this.onWarning,
      warningContainer: warningContainer ?? this.warningContainer,
      onWarningContainer: onWarningContainer ?? this.onWarningContainer,
      decorative: decorative ?? this.decorative,
      onDecorative: onDecorative ?? this.onDecorative,
      decorativeContainer: decorativeContainer ?? this.decorativeContainer,
      onDecorativeContainer: onDecorativeContainer ?? this.onDecorativeContainer,
      decorative2: decorative2 ?? this.decorative2,
      onDecorative2: onDecorative2 ?? this.onDecorative2,
      decorative2Container: decorative2Container ?? this.decorative2Container,
      onDecorative2Container: onDecorative2Container ?? this.onDecorative2Container,
    );
  }

  @override
  CustomColors lerp(ThemeExtension<CustomColors>? other, double t) {
    if (other is! CustomColors) {
      return this;
    }

    return CustomColors(
      success: .lerp(success, other.success, t) ?? success,
      onSuccess: .lerp(onSuccess, other.onSuccess, t) ?? onSuccess,
      successContainer: .lerp(successContainer, other.successContainer, t) ?? successContainer,
      onSuccessContainer: .lerp(onSuccessContainer, other.onSuccessContainer, t) ?? onSuccessContainer,
      warningFixed: .lerp(warningFixed, other.warningFixed, t) ?? warningFixed,
      warning: .lerp(warning, other.warning, t) ?? warning,
      onWarning: .lerp(onWarning, other.onWarning, t) ?? onWarning,
      warningContainer: .lerp(warningContainer, other.warningContainer, t) ?? warningContainer,
      onWarningContainer: .lerp(onWarningContainer, other.onWarningContainer, t) ?? onWarningContainer,
      decorative: .lerp(decorative, other.decorative, t) ?? decorative,
      onDecorative: .lerp(onDecorative, other.onDecorative, t) ?? onDecorative,
      decorativeContainer: .lerp(decorativeContainer, other.decorativeContainer, t) ?? decorativeContainer,
      onDecorativeContainer: .lerp(onDecorativeContainer, other.onDecorativeContainer, t) ?? onDecorativeContainer,
      decorative2: .lerp(decorative2, other.decorative2, t) ?? decorative2,
      onDecorative2: .lerp(onDecorative2, other.onDecorative2, t) ?? onDecorative2,
      decorative2Container: .lerp(decorative2Container, other.decorative2Container, t) ?? decorative2Container,
      onDecorative2Container: .lerp(onDecorative2Container, other.onDecorative2Container, t) ?? onDecorative2Container,
    );
  }
}
