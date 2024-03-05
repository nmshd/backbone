import 'package:admin_api_sdk/admin_api_sdk.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';
import 'package:shared_preferences/shared_preferences.dart';

class SplashScreen extends StatefulWidget {
  const SplashScreen({super.key});

  @override
  State<SplashScreen> createState() => _SplashScreenState();
}

class _SplashScreenState extends State<SplashScreen> {
  @override
  void initState() {
    super.initState();

    route();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Image.asset('assets/icon.png', width: 100, height: 100),
            const SizedBox(height: 40),
            const SizedBox(width: 300, child: LinearProgressIndicator()),
          ],
        ),
      ),
    );
  }

  Future<void> route() async {
    await GetIt.I.allReady();
    final sp = await SharedPreferences.getInstance();

    if (!sp.containsKey('api_key')) {
      if (mounted) context.go('/login');

      return;
    }

    final apiKey = sp.getString('api_key')!;
    const baseUrl = kIsWeb ? '' : String.fromEnvironment('BASE_URL');

    final isValid = await AdminApiClient.validateApiKey(baseUrl: baseUrl, apiKey: apiKey);
    if (!isValid) {
      await sp.remove('api_key');
      if (mounted) context.go('/login');

      return;
    }

    GetIt.I.registerSingleton(await AdminApiClient.create(baseUrl: baseUrl, apiKey: apiKey));
    if (mounted) context.go('/dashboard');
  }
}
