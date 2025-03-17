import 'package:enmeshed_ui_kit/enmeshed_ui_kit.dart';
import 'package:flex_seed_scheme/flex_seed_scheme.dart';
import 'package:flutter/material.dart';

const _successSeedColor = Color(0xFF428C17);
const _warningSeedColor = Color(0xFF8C6117);
const _decorativeSeedColor = Color(0xFF61178C);
const _decorative2SeedColor = Color(0xFF8EB0E9);

final lightCustomColors = CustomColors.generate(
  brightness: Brightness.light,
  tonesConstructor: FlexTones.material,
  successSeedColor: _successSeedColor,
  warningSeedColor: _warningSeedColor,
  decorativeSeedColor: _decorativeSeedColor,
  decorative2SeedColor: _decorative2SeedColor,
);

final darkCustomColors = CustomColors.generate(
  brightness: Brightness.dark,
  tonesConstructor: FlexTones.material,
  successSeedColor: _successSeedColor,
  warningSeedColor: _warningSeedColor,
  decorativeSeedColor: _decorativeSeedColor,
  decorative2SeedColor: _decorative2SeedColor,
);

final lightHighContrastCustomColors = CustomColors.generate(
  brightness: Brightness.light,
  tonesConstructor: FlexTones.ultraContrast,
  successSeedColor: _successSeedColor,
  warningSeedColor: _warningSeedColor,
  decorativeSeedColor: _decorativeSeedColor,
  decorative2SeedColor: _decorative2SeedColor,
);

final darkHighContrastCustomColors = CustomColors.generate(
  brightness: Brightness.dark,
  tonesConstructor: FlexTones.ultraContrast,
  successSeedColor: _successSeedColor,
  warningSeedColor: _warningSeedColor,
  decorativeSeedColor: _decorativeSeedColor,
  decorative2SeedColor: _decorative2SeedColor,
);
