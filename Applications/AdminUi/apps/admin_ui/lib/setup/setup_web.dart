import 'package:flutter_web_plugins/url_strategy.dart';
import 'package:get_it/get_it.dart';

import '/core/app_config.dart';

Future<void> setup() async {
  GetIt.I.registerSingleton<AppConfig>(AppConfig(baseUrl: ''));

  usePathUrlStrategy();
}
