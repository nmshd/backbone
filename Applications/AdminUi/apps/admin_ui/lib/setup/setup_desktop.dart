import 'dart:io';
import 'dart:ui';

import 'package:get_it/get_it.dart';
import 'package:window_size/window_size.dart';

import '/core/core.dart';

Future<void> setup() async {
  if (Platform.isWindows || Platform.isLinux || Platform.isMacOS) {
    setWindowMinSize(const Size(1200, 800));
  }

  GetIt.I.registerSingleton<AppConfig>(AppConfig(baseUrl: const String.fromEnvironment('base_url')));
}
