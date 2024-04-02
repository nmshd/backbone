import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';

import '/theme/theme.dart';

extension UnregisterIfRegistered on GetIt {
  Future<void> unregisterIfRegistered<T extends Object>() async {
    if (!isRegistered<T>()) return;

    await unregister<T>();
  }
}

extension GetCustomColors on BuildContext {
  CustomColors get customColors => Theme.of(this).extension<CustomColors>()!;
}
