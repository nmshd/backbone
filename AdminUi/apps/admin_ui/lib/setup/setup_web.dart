import 'package:get_it/get_it.dart';

import '/utils/app_config.dart';

Future<void> setup() async {
  GetIt.I.registerSingleton<AppConfig>(AppConfig(baseUrl: ''));
}
