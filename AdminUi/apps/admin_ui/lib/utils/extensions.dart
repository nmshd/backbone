import 'package:get_it/get_it.dart';

extension UnregisterIfRegistered on GetIt {
  Future<void> unregisterIfRegistered<T extends Object>() async {
    if (!isRegistered<T>()) return;

    await unregister<T>();
  }
}
