import 'dart:io';
import 'dart:ui';

import 'package:window_size/window_size.dart';

Future<void> setup() async {
  if (Platform.isWindows || Platform.isLinux || Platform.isMacOS) {
    setWindowMinSize(const Size(1200, 800));
  }
}
